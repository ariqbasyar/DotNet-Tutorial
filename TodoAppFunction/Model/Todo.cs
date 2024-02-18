using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace TodoAppFunction.Model
{
    public class Todo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty(
            "todoType",
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue("Uncategorized")]
        public string? TodoType { get; set; }

        [JsonProperty("isComplete")]
        public bool? IsComplete { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        //[JsonConstructor]
        public Todo() { 
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
            IsComplete = false;
        }

        public Todo Update(Todo t)
        {
            if (t == null) return this;
            Title = t.Title ?? Title;
            TodoType = t.TodoType ?? TodoType;
            IsComplete = t.IsComplete ?? IsComplete;
            ModifiedDate = DateTime.Now;
            return this;
        }
    }
}
