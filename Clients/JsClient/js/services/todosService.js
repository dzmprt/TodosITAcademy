const todosKey = 'todosKey';

function addTodo(todo) {
    let todos = getTodosFromLocalStorage();
    todos.push(todo);
    updateTodosInLocalStorage(todos);
}

function deleteTodo(todoId) {
    let todos = getTodosFromLocalStorage();
    todos = todos.filter(f => f.todoId != todoId);
    updateTodosInLocalStorage(todos);
}

function updateTodo(todo) {
    let todos = getTodosFromLocalStorage();
    if (todos.find(f => f.todoId == todo.todoId) === undefined) {
        throw "Todo don't exists in local storage";
    }
    todo.updatedDate = new Date();
    todos = todos.filter(f => f.todoId != todo.todoId);
    todos.push(todo);
    updateTodosInLocalStorage(todos);
}

function getTodo(todoId) {
    var todos = getTodosFromLocalStorage().filter(t => t.todoId == todoId);
    if (todos.length == 0) {
        return null;
    } else {
        return todos[0];
    }
}

function getTodos() {
    var todos = getTodosFromLocalStorage().sort((todo1, todo2) => {
        if (todo1.isDone < todo2.isDone) return -1;
        if (todo1.isDone > todo2.isDone) return 1;
        if (todo1.updatedDate < todo2.updatedDate) return 1;
        if (todo1.updatedDate > todo2.updatedDate) return -1;
        return 0;
    });

    return todos;
}


function innitTodosCollection() {
    let todos = localStorage.getItem(todosKey);
    if (todos === null) {
        localStorage.setItem(todosKey, JSON.stringify([]));
    }
}

function getTodosFromLocalStorage() {
    innitTodosCollection();
    let todosJson = localStorage.getItem(todosKey);
    return JSON.parse(todosJson);
}

function updateTodosInLocalStorage(todos) {
    localStorage.setItem(todosKey, JSON.stringify(todos));
}
