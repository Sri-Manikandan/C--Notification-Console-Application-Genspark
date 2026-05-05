# Project Retrospective: Simple Notification System (SNS)

## Overview

This is a C# console application that manages users and sends Email/SMS notifications. The project covers user CRUD operations through a menu-driven interface and delivers messages to all registered users via both email and SMS channels.

**Project structure:**

- `Models/` — Notification, EmailNotification, SmsNotification, User
- `Interfaces/` — INotificationService, IRepository<T,K>
- `Services/` — NotificationService
- `Repositories/` — UserRepository
- `Program.cs` — Console menu loop

---

## 1. Abstract Classes

### What Did Not Go Well

`Notification` is defined as a plain `public class`, but it should be `abstract`. A bare `Notification` object without a delivery channel (Email or SMS) makes no sense, yet the current design allows this:

```csharp
// Models/Notification.cs:11 — can be instantiated directly, which is wrong
public class Notification {
    public Notification(DateTime sentTime, string message, NotificationType notificationType) { ... }
}
```

The deeper problem is that `EmailNotification` and `SmsNotification` do all their work inside the constructor:

```csharp
// Models/EmailNotification.cs:5 — side effect in constructor
public EmailNotification(DateTime sentTime, string message, string userName)
    : base(sentTime, message, NotificationType.Email) {
    Console.WriteLine("Email Sent to " + userName + " : " + Message);
}
```

Objects should deliver via methods, not during construction. A constructor should initialize state, nothing more.

### What It Should Look Like

Making `Notification` abstract with a `Send()` method forces every subclass to define how it delivers the message. The compiler enforces this — if you add a `PushNotification` and forget to implement `Send()`, it will not compile.

```csharp
public abstract class Notification {
    public DateTime SentTime { get; set; }
    public string Message { get; set; }
    public NotificationType NotificationType { get; set; }

    protected Notification(DateTime sentTime, string message, NotificationType type) {
        SentTime = sentTime;
        Message = message;
        NotificationType = type;
    }

    // Every subclass must define how it sends
    public abstract void Send(User recipient);

    public override string ToString() =>
        $"[{NotificationType}] {Message} at {SentTime}";
}

public class EmailNotification : Notification {
    public EmailNotification(DateTime sentTime, string message)
        : base(sentTime, message, NotificationType.Email) { }

    public override void Send(User recipient) {
        Console.WriteLine($"Email sent to {recipient.Email}: {Message}");
    }
}

public class SmsNotification : Notification {
    public SmsNotification(DateTime sentTime, string message)
        : base(sentTime, message, NotificationType.SMS) { }

    public override void Send(User recipient) {
        Console.WriteLine($"SMS sent to {recipient.PhoneNumber}: {Message}");
    }
}
```

And in `NotificationService`, instead of constructing and throwing away objects:

```csharp
// Current — object created purely for constructor side effect, then discarded
new EmailNotification(DateTime.Now, message, user.Name);

// Better — object does its work through a method
Notification email = new EmailNotification(DateTime.Now, message);
email.Send(user);
```

---

## 2. Partial Classes

### What Did Not Go Well

Partial classes were not used at all. For a small project that is acceptable, but `NotificationService` is doing two completely unrelated jobs in one class — user management and notification delivery. This violates the Single Responsibility Principle.

```csharp
// Services/NotificationService.cs — two concerns in one file
internal class NotificationService : INotificationService {
    // User management
    public User AddUser(...) { ... }
    public List<User> GetAllUsers() { ... }
    public User? GetUser(string email) { ... }
    public User? UpdateUser(...) { ... }
    public User? DeleteUser(string email) { ... }

    // Notification delivery
    public void sendMessage(string message) { ... }
}
```

### What It Should Look Like

Partial classes let the compiler merge multiple files into one class at build time while keeping your code physically organized by concern. This is especially useful when one part grows large or when auto-generated code needs to live alongside hand-written code.

```csharp
// NotificationService.Users.cs — user management only
internal partial class NotificationService {
    private readonly IRepository<User, string> _userRepository;

    public NotificationService(IRepository<User, string> userRepository) {
        _userRepository = userRepository;
    }

    public User AddUser(string name, string email, string phoneNumber) { ... }
    public List<User> GetAllUsers() { ... }
    public User? GetUser(string email) { ... }
    public User? UpdateUser(string email, string name, string phone) { ... }
    public User? DeleteUser(string email) { ... }
}

// NotificationService.Messaging.cs — notification delivery only
internal partial class NotificationService : INotificationService {
    public void sendMessage(string message) {
        foreach (var user in _userRepository.GetAll()) {
            new EmailNotification(DateTime.Now, message).Send(user);
            new SmsNotification(DateTime.Now, message).Send(user);
        }
    }
}
```

---

## 3. Model Preparation

### What Went Well

- Models are cleanly separated into their own `Models/` folder.
- `NotificationType` as an enum is a good extensibility choice — adding `Push` later requires just one new enum value.
- Both `User` and `Notification` override `ToString()`, which is helpful for debugging and console output.
- The `User` constructor sets `IsActive = true` by default, which is a sensible default.

### What Did Not Go Well

**User and Notification are not linked.**

A `Notification` has no reference to who it was sent to. The recipient is passed as a plain `string userName`, which loses type safety and any ability to access other user properties like the email address or phone number.

```csharp
// Models/EmailNotification.cs:5 — userName is just a string
public EmailNotification(DateTime sentTime, string message, string userName)
```

If you later want to know which notifications were sent to which user — for an audit log or a resend feature — you have no way to trace that. The `Notification` model should hold a typed reference to its recipient.

```csharp
public abstract class Notification {
    public User Recipient { get; set; }   // typed, not a loose string
    public DateTime SentTime { get; set; }
    public string Message { get; set; }
}
```

**Inconsistent access modifiers.**

`Notification` is `public` while `User`, `EmailNotification`, and `SmsNotification` are all `internal`. Since this is a single-assembly console app, `internal` is appropriate everywhere. The inconsistency suggests access levels were not intentionally chosen.

```csharp
public class Notification { ... }       // public
internal class User { ... }             // internal — inconsistent
internal class EmailNotification { }    // internal
```

**Soft-delete that does not actually work.**

In `UserRepository.Delete()`, the code marks a user as inactive and then immediately removes them from the dictionary:

```csharp
// Repositories/UserRepository.cs:36-40
User user = _users[email];
user.IsActive = false;      // sets the flag...
_users.Remove(email);       // ...then removes the record
return user;
```

The returned object has `IsActive = false`, but since the user is gone from storage, this flag is never observable again. Pick one approach and commit to it:

- **Soft-delete:** Keep the user in the dictionary, mark `IsActive = false`, and filter inactive users from `GetAll()`.
- **Hard-delete:** Remove the user entirely, do not touch `IsActive`.

**No input validation.**

`User` accepts any string as an email or phone number. The model has no guard against empty strings or malformed data.

---

## 4. Interface Design

### What Went Well

`IRepository<T, K>` is the strongest design decision in the project. It is generic, constrained to reference types, and covers all CRUD operations cleanly:

```csharp
// Interfaces/IRepository.cs
internal interface IRepository<T, K> where T : class {
    T Add(T entity);
    List<T> GetAll();
    T? Get(K key);
    T? Update(K key, T entity);
    T? Delete(K key);
}
```

`UserRepository` implements this correctly, and the indexer `this[string email]` is a nice C# touch for direct lookup.

### What Did Not Go Well

`INotificationService` only exposes one method:

```csharp
// Interfaces/NotificationInterface.cs
internal interface INotificationService {
    public void sendMessage(string Message);
}
```

But `NotificationService` has six public methods. If `Program.cs` were to reference the service through the interface — which is the entire point of coding to an interface — all user management methods would be invisible. The interface barely represents the contract.

A better split:

```csharp
internal interface IUserService {
    User AddUser(string name, string email, string phoneNumber);
    List<User> GetAllUsers();
    User? GetUser(string email);
    User? UpdateUser(string email, string name, string phoneNumber);
    User? DeleteUser(string email);
}

internal interface INotificationService {
    void SendMessage(string message);
}

// The service implements both
internal class NotificationService : IUserService, INotificationService { ... }
```

**Hard-wired dependency.**

`NotificationService` constructs its own repository directly:

```csharp
// Services/NotificationService.cs:9
private UserRepository _userRepository = new UserRepository();
```

This locks the service to one concrete implementation. Using constructor injection instead means you can swap in a database-backed repository later without touching `NotificationService` at all:

```csharp
private readonly IRepository<User, string> _userRepository;

public NotificationService(IRepository<User, string> userRepository) {
    _userRepository = userRepository;
}
```

---

## 5. Program.cs

### What Went Well

- The menu loop is readable and logically organized.
- Null-coalescing `?? string.Empty` handles `Console.ReadLine()` returning null safely.

### What Did Not Go Well

`Convert.ToInt32(Console.ReadLine())` throws a `FormatException` if the user types anything other than a number. This crashes the entire program.

```csharp
// Program.cs:25 — will throw on "abc" or an empty enter
int choice = Convert.ToInt32(Console.ReadLine());
```

Use `int.TryParse` and loop back if the input is invalid:

```csharp
if (!int.TryParse(Console.ReadLine(), out int choice)) {
    Console.WriteLine("Invalid input. Please enter a number.");
    continue;
}
```

---

## Key Takeaways

- Make `Notification` abstract and add an abstract `Send(User recipient)` method — this is the single biggest structural improvement available.
- Move delivery logic out of constructors and into methods.
- Use partial classes to split `NotificationService` by concern.
- Link `Notification` to `User` with a typed reference, not a string.
- Expand `INotificationService` to cover the full service contract, or split it into two interfaces.
- Use constructor injection so `NotificationService` depends on `IRepository<User, string>`, not `UserRepository` directly.
- Replace `Convert.ToInt32` with `int.TryParse` to prevent crashes on bad input.
- Pick one delete strategy — soft or hard — and be consistent.
