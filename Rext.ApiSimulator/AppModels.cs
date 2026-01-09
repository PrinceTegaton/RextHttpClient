namespace Rext.ApiSimulator
{
    public class Result
    {
        public string? Message { get; set; }

        public Result()
        {

        }

        public Result(string msg)
        {
            Message = msg;
        }
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }

        public Result()
        {

        }
        
        public Result(T data, string msg)
        {
            Data = data;
            Message = msg;
        }
    }

    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Contact? Contact { get; set; }
        public IEnumerable<ChangeHistory> ChangeHistory { get; set; } = [];
    }
    public class Contact
    {
        public string? EmailAddress { get; set; }
        public string? MobileNumber { get; set; }
    }
    public class ChangeHistory
    {
        public int Id { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class AppDataset
    {
        public static IList<User> Users => [
            new() {
                Id = 1,
                Name = "Marko",
                DateOfBirth = DateOnly.Parse("1987-01-05"),
                Contact = new() {
                    EmailAddress = "marko101@mailbox.com",
                    MobileNumber = "09917676712"
                },
                ChangeHistory = [
                    new() { Id = 1, Note = "User profile created", CreatedOn = DateTime.Parse("2020-01-15T10:00:00") },
                    new() { Id = 2, Note = "Email address updated", CreatedOn = DateTime.Parse("2021-06-20T14:30:00") },
                    new() { Id = 3, Note = "Mobile number added", CreatedOn = DateTime.Parse("2022-03-10T09:15:00") },
                    new() { Id = 4, Note = "Date of birth verified", CreatedOn = DateTime.Parse("2023-11-05T16:45:00") }
                ]
            },
            new() {
                Id = 2,
                Name = "Anna",
                DateOfBirth = DateOnly.Parse("1990-03-12"),
                Contact = new() {
                    EmailAddress = "anna.b@example.com",
                    MobileNumber = "01234567890"
                },
                ChangeHistory = [
                    new() { Id = 5, Note = "Initial registration", CreatedOn = DateTime.Parse("2019-08-22T11:20:00") },
                    new() { Id = 6, Note = "Name updated to Anna", CreatedOn = DateTime.Parse("2020-12-01T13:10:00") },
                    new() { Id = 7, Note = "Contact details confirmed", CreatedOn = DateTime.Parse("2021-09-18T15:00:00") }
                ]
            },
            new() {
                Id = 3,
                Name = "Carlos",
                DateOfBirth = DateOnly.Parse("1985-07-20"),
                Contact = new() {
                    EmailAddress = "carlos@domain.net",
                    MobileNumber = "09876543210"
                },
                ChangeHistory = [
                    new() { Id = 8, Note = "Account setup", CreatedOn = DateTime.Parse("2021-02-14T08:45:00") },
                    new() { Id = 9, Note = "Email changed", CreatedOn = DateTime.Parse("2022-07-30T12:25:00") },
                    new() { Id = 10, Note = "Mobile number updated", CreatedOn = DateTime.Parse("2024-01-12T17:20:00") },
                    new() { Id = 11, Note = "Profile reviewed", CreatedOn = DateTime.Parse("2025-05-08T10:30:00") }
                ]
            },
            new() {
                Id = 4,
                Name = "Diana",
                DateOfBirth = DateOnly.Parse("1992-11-08"),
                Contact = new() {
                    EmailAddress = "diana.smith@email.com",
                    MobileNumber = "07788990011"
                },
                ChangeHistory = [
                    new() { Id = 12, Note = "User added", CreatedOn = DateTime.Parse("2020-04-03T09:50:00") },
                    new() { Id = 13, Note = "DOB corrected", CreatedOn = DateTime.Parse("2021-10-19T14:00:00") }
                ]
            },
            new() {
                Id = 5,
                Name = "Ethan",
                DateOfBirth = DateOnly.Parse("1979-05-15"),
                Contact = new() {
                    EmailAddress = "ethan.wilson@outlook.com",
                    MobileNumber = "05556667778"
                },
                ChangeHistory = [
                    new() { Id = 14, Note = "Initial entry", CreatedOn = DateTime.Parse("2018-11-27T16:15:00") },
                    new() { Id = 15, Note = "Contact info updated", CreatedOn = DateTime.Parse("2020-03-05T11:40:00") },
                    new() { Id = 16, Note = "Name verified", CreatedOn = DateTime.Parse("2022-08-22T13:55:00") },
                    new() { Id = 17, Note = "Recent audit", CreatedOn = DateTime.Parse("2025-12-01T09:00:00") }
                ]
            },
            new() {
                Id = 6,
                Name = "Fiona",
                DateOfBirth = DateOnly.Parse("1988-09-22"),
                Contact = new() {
                    EmailAddress = "fiona@techmail.org",
                    MobileNumber = "04445556667"
                },
                ChangeHistory = [
                    new() { Id = 18, Note = "Profile creation", CreatedOn = DateTime.Parse("2021-07-11T12:10:00") },
                    new() { Id = 19, Note = "Mobile added", CreatedOn = DateTime.Parse("2023-02-28T15:30:00") },
                    new() { Id = 20, Note = "Email updated", CreatedOn = DateTime.Parse("2024-09-15T10:20:00") }
                ]
            },
            new() {
                Id = 7,
                Name = "Gabriel",
                DateOfBirth = DateOnly.Parse("1995-02-14"),
                Contact = new() {
                    EmailAddress = "gabriel.jones@yahoo.com",
                    MobileNumber = "03334445556"
                }
            },
            new() {
                Id = 8,
                Name = "Hannah",
                DateOfBirth = DateOnly.Parse("1983-12-30"),
                Contact = new() {
                    EmailAddress = "hannah.miller@gmx.com",
                    MobileNumber = "02223334445"
                }
            },
            new() {
                Id = 9,
                Name = "Igor",
                DateOfBirth = DateOnly.Parse("1991-06-03"),
                Contact = new() {
                    EmailAddress = "igor@protonmail.ch",
                    MobileNumber = "01112223334"
                }
            },
            new() {
                Id = 10,
                Name = "Julia",
                DateOfBirth = DateOnly.Parse("1986-04-17"),
                Contact = new() {
                    EmailAddress = "julia.brown@zmail.io",
                    MobileNumber = "06667778889"
                }
            }
        ];
    }
}