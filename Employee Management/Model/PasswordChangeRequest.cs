namespace Employee_Management.Model
{
    public class PasswordChangeRequest
    {
        public string UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
