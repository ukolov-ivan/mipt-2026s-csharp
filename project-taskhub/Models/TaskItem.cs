namespace TaskHub.Models
{
    public record TaskItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Priority Priority { get; set; }
        public DateTime Deadline { get; set; }
        public Status Status { get; set; }

        public override string ToString()
        {
            return $"{Name} [{Id}] | Priority: {Priority} | Status: {Status} | Deadline: {Deadline:yyyy-MM-dd HH:mm}";
        }
    }
}
