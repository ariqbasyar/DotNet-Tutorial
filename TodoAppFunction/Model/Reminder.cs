using Newtonsoft.Json;
using System;

namespace TodoAppFunction.Model
{
    public class Reminder
    {
        [JsonProperty("id")]
        public string Id {  get; set; }

        [JsonProperty("todoId")]
        public string TodoId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("todoType")]
        public string TodoType { get; set; }

        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? ModifiedTime { get; set; }

        public Reminder() {
            Id = Guid.NewGuid().ToString();
            CreatedTime = DateTime.Now;
        }

        public static Reminder CreateFrom(Todo todo)
        {
            return new Reminder()
            {
                TodoId = todo.Id,
                Title = todo.Title,
                TodoType = todo.TodoType,
                IsCompleted = todo.IsComplete ?? false
            };
        }
    }
}
