using System;
using Sns.Services;
using Sns.Models;

namespace Sns{
    class Program{
        static void Main(string[] args){
            NotificationService notificationService = new NotificationService();

            while(true){
                Console.WriteLine("\n--- User Management Menu ---");
                Console.WriteLine("1. Add User");
                Console.WriteLine("2. List All Users");
                Console.WriteLine("3. Get User by Email");
                Console.WriteLine("4. Update User");
                Console.WriteLine("5. Delete User");
                Console.WriteLine("6. Send Message to All");
                Console.WriteLine("7. Exit");
                Console.Write("Enter choice: ");

                int choice = Convert.ToInt32(Console.ReadLine());

                switch(choice){
                    case 1:
                        Console.Write("Enter Name: ");
                        string name = Console.ReadLine() ?? string.Empty;
                        Console.Write("Enter Email: ");
                        string email = Console.ReadLine() ?? string.Empty;
                        Console.Write("Enter Phone Number: ");
                        string phone = Console.ReadLine() ?? string.Empty;
                        User created = notificationService.AddUser(name, email, phone);
                        Console.WriteLine("Created: " + created);
                        break;

                    case 2:
                        var users = notificationService.GetAllUsers();
                        if(users.Count == 0){
                            Console.WriteLine("No users found.");
                        } else {
                            foreach(var u in users)
                                Console.WriteLine(u + "\n");
                        }
                        break;

                    case 3:
                        Console.Write("Enter Email: ");
                        string searchEmail = Console.ReadLine() ?? string.Empty;
                        User? found = notificationService.GetUser(searchEmail);
                        Console.WriteLine(found != null ? found.ToString() : "User not found.");
                        break;

                    case 4:
                        Console.Write("Enter Email of user to update: ");
                        string updateEmail = Console.ReadLine() ?? string.Empty;
                        Console.Write("Enter new Name: ");
                        string newName = Console.ReadLine() ?? string.Empty;
                        Console.Write("Enter new Phone Number: ");
                        string newPhone = Console.ReadLine() ?? string.Empty;
                        User? updated = notificationService.UpdateUser(updateEmail, newName, newPhone);
                        Console.WriteLine(updated != null ? "Updated: " + updated : "User not found.");
                        break;

                    case 5:
                        Console.Write("Enter Email of user to delete: ");
                        string deleteEmail = Console.ReadLine() ?? string.Empty;
                        User? deleted = notificationService.DeleteUser(deleteEmail);
                        Console.WriteLine(deleted != null ? "Deleted: " + deleted : "User not found.");
                        break;

                    case 6:
                        Console.Write("Enter Message: ");
                        string message = Console.ReadLine() ?? string.Empty;
                        notificationService.sendMessage(message);
                        break;

                    case 7:
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }
}
