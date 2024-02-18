﻿using Newtonsoft.Json;
using System;

namespace TodoAppFunction.Model
{
    public class TodoHistory
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("todoId")]
        public string TodoId { get; set; }

        [JsonProperty("title")]
        public string Title {  get; set; }

        [JsonProperty("todoType")]
        public string TodoType { get; set; }

        [JsonProperty("isComplete")]
        public bool? IsComplete { get; set; }

        public DateTime CreatedDate { get; set; }

        public TodoHistory() { 
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
        }

        public static TodoHistory CreateFrom(Todo todo)
        {
            TodoHistory todoHistory = new();
            todoHistory.TodoId = todo.Id;
            todoHistory.Title = todo.Title;
            todoHistory.TodoType = todo.TodoType;
            todoHistory.IsComplete = todo.IsComplete;
            return todoHistory;
        }
    }
}
