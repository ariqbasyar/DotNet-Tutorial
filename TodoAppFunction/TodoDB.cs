using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoAppFunction
{
    internal sealed class TodoDB
    {
        private TodoDB() { }
        private static TodoDB instance;
        public static TodoDB Instance
        {
            get
            {
                instance ??= new TodoDB();
                return instance;
            }
        }

        private List<Todo> todos = [new Todo("tidur", "gabut", true), new Todo("ngoding", "ngantor", false), new Todo("ngepel", "pekerjaan rumah", false)];
        public List<Todo> Get() { return todos; }
        public Todo Add(Todo todo)
        {
            todos.Add(todo);
            return todo;
        }

        public Todo? Put(int id, Todo todoInput)
        {
            foreach (Todo todo in todos)
            {
                if (todo.Id != id) continue;
                todo.Update(todoInput);
                return todo;
            }
            return null;
        }

        public Todo? Delete(int id)
        {
            for (int i = 0; i < todos.Count; i++)
            {
                if (todos[i].Id != id) continue;
                Todo temp = todos[i];
                todos.RemoveAt(i);
                return temp;
            }
            return null;
        }
    }
}
