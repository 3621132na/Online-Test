import {useState} from 'react';
import {Routes, Route} from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import Tasks from './components/Tasks';

function App() {
    const [userInfo, setUserInfo] = useState(null);
    const setToken = (info) => {
        setUserInfo(info);
    };
    const handleLogout = () => {
        localStorage.removeItem('token');
        setUserInfo(null);
    };
    return (
        <div className="App">
            <Routes>
                <Route path="/login" element={<Login setToken={setToken} setView={() => {
                }}/>}/>
                <Route path="/register" element={<Register setToken={setToken} setView={() => {
                }}/>}/>
                <Route path="/"
                       element={<Tasks token={userInfo?.token} role={userInfo?.role} setToken={handleLogout}/>}/>
            </Routes>
        </div>
    );
}
export default App;