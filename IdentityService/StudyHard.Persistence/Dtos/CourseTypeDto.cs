namespace StudyHard.Persistence.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PictureUrl { get; set; }
    }

    public class CourseTypeDto
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public string Type { get; set; }
    }
}
