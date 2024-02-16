using System.Text.Json.Serialization;

namespace TodoAppFunction
{
    class Todo
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        public string? TodoType { get; set; }

        public bool IsComplete { get; set; }

        private static Random rnd = new Random();

        public Todo() { Id = rnd.Next(); }
        [JsonConstructor]
        public Todo(string title) : this() { Title = title; }
        public Todo(string title, string type) : this(title) { TodoType = type; }
        public Todo(string title, string type, bool isComplete) : this(title, type) { IsComplete = isComplete; }
        public Todo Update(Todo todo)
        {
            Title = todo.Title;
            TodoType = todo.TodoType;
            IsComplete = todo.IsComplete;
            return this;
        }
    }
}
