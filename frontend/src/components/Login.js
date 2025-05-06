import {useState} from 'react';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';
import { useNavigate } from 'react-router-dom';
const API_URL = 'http://localhost:5170/api';

function Login({setToken, setView}) {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();
    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post(`${API_URL}/auth/login`, { username, password });
            const token = response.data.token;
            localStorage.setItem('token', token);

            const decodedToken = jwtDecode(token);
            const userInfo = {
                token: token,
                role: decodedToken[ClaimTypes.Role] || 'Regular',
                userId: decodedToken[ClaimTypes.NameIdentifier]
            };

            setToken(userInfo);
            setError('');
            navigate("/"); // ✅ chuyển sang trang chủ sau khi login
        } catch (err) {
            setError('Invalid credentials');
        }
    };

    return (
        <div className="max-w-md mx-auto mt-10 p-6 bg-white rounded-lg shadow-lg">
            <h2 className="text-2xl font-bold mb-4">Login</h2>
            <form onSubmit={handleSubmit}>
                <div className="mb-4">
                    <label className="block text-gray-700">Username</label>
                    <input
                        name="username"
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        className="w-full p-2 border rounded"
                        required
                    />
                </div>
                <div className="mb-4">
                    <label className="block text-gray-700">Password</label>
                    <input
                        name="password"
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        className="w-full p-2 border rounded"
                        required
                    />
                </div>
                {error && <p className="text-red-500">{error}</p>}
                <button type="submit" className="w-full bg-blue-500 text-white p-2 rounded hover:bg-blue-600">
                    Login
                </button>
            </form>
            <p className="mt-4">
                Don't have an account?{' '}
                <a href="#" onClick={() => navigate('/register')} className="text-blue-500">
                    Register
                </a>
            </p>
        </div>
    );
}

const ClaimTypes = {
    Role: 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
    NameIdentifier: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
};
export default Login;