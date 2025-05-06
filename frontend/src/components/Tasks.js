import {useState, useEffect} from 'react';
import axios from 'axios';
import {useNavigate} from 'react-router-dom';

const API_URL = 'http://localhost:5170/api';

function Tasks({token, setToken}) {
    const [tasks, setTasks] = useState([]);
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [status, setStatus] = useState('ToDo');
    const [dueDate, setDueDate] = useState('');
    const [filterStatus, setFilterStatus] = useState('');
    const [sortOrder, setSortOrder] = useState('asc');
    const [editingTask, setEditingTask] = useState(null);
    const navigate = useNavigate();
    const [keyword, setKeyword] = useState('');
    const [welcomeMessage, setWelcomeMessage] = useState('');
    useEffect(() => {
        fetchTasks();
    }, [filterStatus, sortOrder, keyword]);
    useEffect(() => {
        if (token) {
            setWelcomeMessage('Welcome to Task Management!');
        }
    }, [token]);
    const fetchTasks = async () => {
        if (!token) {
            handleLogout();
            return;
        }
        try {
            const response = await axios.get(`${API_URL}/tasks`, {
                headers: {Authorization: `Bearer ${token}`}, params: {status: filterStatus, sortOrder, keyword},
            });
            setTasks(response.data);
            console.log("Keyword: ", keyword);
        } catch (err) {
            console.error('Failed to fetch tasks', err);
            if (err.response?.status === 401) {
                handleLogout();
            }
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            await axios.post(`${API_URL}/tasks`, {
                title,
                description,
                status,
                dueDate
            }, {headers: {Authorization: `Bearer ${token}`}});
            fetchTasks();
            setTitle('');
            setDescription('');
            setStatus('ToDo');
            setDueDate('');
        } catch (err) {
            console.error('Failed to create task', err);
        }
    };

    const handleEdit = (task) => {
        setEditingTask(task);
        setTitle(task.title);
        setDescription(task.description || '');
        setStatus(task.status);
        setDueDate(task.duedate.split('T')[0]);
    };

    const handleUpdate = async (e) => {
        e.preventDefault();
        try {
            await axios.put(`${API_URL}/tasks/${editingTask.id}`, {
                title,
                description,
                status,
                dueDate
            }, {headers: {Authorization: `Bearer ${token}`}});
            fetchTasks();
            setEditingTask(null);
            setTitle('');
            setDescription('');
            setStatus('ToDo');
            setDueDate('');
        } catch (err) {
            console.error('Failed to update task', err);
        }
    };

    const handleCancelEdit = () => {
        setEditingTask(null);
        setTitle('');
        setDescription('');
        setStatus('ToDo');
        setDueDate('');
    };

    const handleDelete = async (id) => {
        try {
            await axios.delete(`${API_URL}/tasks/${id}`, {
                headers: {Authorization: `Bearer ${token}`},
            });
            fetchTasks();
        } catch (err) {
            console.error('Failed to delete task', err);
        }
    };

    const handleLogout = () => {
        localStorage.removeItem('token');
        setToken(null);
        navigate('/login'); // 👉 chuyển hướng về trang login
    };

    return (<div className="max-w-4xl mx-auto mt-10 p-6 bg-white rounded-lg shadow-lg">
        {welcomeMessage && <div className="text-green-500 text-xl mb-4">{welcomeMessage}</div>}
        <div className="flex justify-between items-center mb-6"><h2 className="text-2xl font-bold">Task
            Management</h2>
            <button onClick={handleLogout} className="bg-red-500 text-white p-2 rounded hover:bg-red-600"> Logout
            </button>
        </div>
        <form onSubmit={editingTask ? handleUpdate : handleCreate} className="mb-6">
            <div className="grid grid-cols-2 gap-4">
                <div><label className="block text-gray-700">Title</label> <input name="title" type="text"
                                                                                 value={title}
                                                                                 onChange={(e) => setTitle(e.target.value)}
                                                                                 className="w-full p-2 border rounded"
                                                                                 required/></div>
                <div><label className="block text-gray-700">Due Date</label> <input name="dueDate" type="date"
                                                                                    value={dueDate}
                                                                                    onChange={(e) => setDueDate(e.target.value)}
                                                                                    className="w-full p-2 border rounded"
                                                                                    required/></div>
                <div><label className="block text-gray-700">Status</label> <select name="status" value={status}
                                                                                   onChange={(e) => setStatus(e.target.value)}
                                                                                   className="w-full p-2 border rounded">
                    <option value="ToDo">To Do</option>
                    <option value="InProgress">In Progress</option>
                    <option value="Completed">Completed</option>
                </select></div>
                <div><label className="block text-gray-700">Description</label> <textarea name="description"
                                                                                          value={description}
                                                                                          onChange={(e) => setDescription(e.target.value)}
                                                                                          className="w-full p-2 border rounded"></textarea>
                </div>
            </div>
            <div className="mt-4 flex gap-2">
                <button type="submit"
                        className="bg-blue-500 text-white p-2 rounded hover:bg-blue-600"> {editingTask ? 'Update Task' : 'Add Task'} </button>
                {editingTask && (<button type="button" onClick={handleCancelEdit}
                                         className="bg-gray-500 text-white p-2 rounded hover:bg-gray-600"> Cancel </button>)}
            </div>
        </form>
        <div className="mb-6 flex gap-4">
            <div>
                <label className="block text-gray-700">Search by Keyword</label>
                <input
                    type="text"
                    name="keyword"
                    value={keyword}
                    onChange={(e) => setKeyword(e.target.value)}
                    className="p-2 border rounded"
                    placeholder="Enter keyword"
                />
            </div>
            <button
                onClick={fetchTasks}
                className="bg-blue-500 text-white p-2 rounded hover:bg-blue-600"
            >
                Search
            </button>
            <div><label className="block text-gray-700">Filter by Status</label> <select name="filterStatus"
                                                                                         value={filterStatus}
                                                                                         onChange={(e) => setFilterStatus(e.target.value)}
                                                                                         className="p-2 border rounded">
                <option value="">All</option>
                <option value="ToDo">To Do</option>
                <option value="InProgress">In Progress</option>
                <option value="Completed">Completed</option>
            </select></div>
            <div><label className="block text-gray-700">Sort by Due Date</label> <select name="sortOrder"
                                                                                         value={sortOrder}
                                                                                         onChange={(e) => setSortOrder(e.target.value)}
                                                                                         className="p-2 border rounded">
                <option value="asc">Ascending</option>
                <option value="desc">Descending</option>
            </select></div>
        </div>
        <div className="grid gap-4"> {tasks.map((task) => (
            <div key={task.id} className="p-4 bg-gray-100 rounded-lg flex justify-between items-center">
                <div><h3 className="text-lg font-semibold">{task.title}</h3> <p>{task.description}</p>
                    <p>Status: {task.status}</p> <p>Due: {new Date(task.duedate).toLocaleDateString()}</p></div>
                <div className="flex gap-2">
                    <button onClick={() => handleEdit(task)}
                            className="bg-yellow-500 text-white p-2 rounded hover:bg-yellow-600"> Edit
                    </button>
                    <button onClick={() => handleDelete(task.id)}
                            className="bg-red-500 text-white p-2 rounded hover:bg-red-600"> Delete
                    </button>
                </div>
            </div>))} </div>
    </div>);
}

export default Tasks;