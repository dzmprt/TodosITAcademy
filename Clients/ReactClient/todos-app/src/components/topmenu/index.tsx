import { Link, NavLink } from 'react-router-dom';
import './index.css';

export default function TopHeader() 
{
    return (
        <header className="nav-bar">
                <NavLink className={({ isActive, isPending }) =>
                      isActive
                        ? "active"
                        : isPending
                        ? "pending"
                        : ""
                    } to={`/`}>Home</NavLink>
                <NavLink className={({ isActive, isPending }) =>
                      isActive
                        ? "active"
                        : isPending
                        ? "pending"
                        : ""
                    } to={`/about`}>About</NavLink>
                <div className="separator"></div>
                <NavLink className={({ isActive, isPending }) =>
                      isActive
                        ? "active"
                        : isPending
                        ? "pending"
                        : ""
                    } to={`/sing-in`}>Sing-in</NavLink>
        </header>
    );
}