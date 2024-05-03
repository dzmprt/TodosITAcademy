import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import './index.css'
import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import About from './pages/About/index.tsx';
import Auth from './pages/Auth/index.tsx';
import Home from './pages/Home/index.tsx';

const router = createBrowserRouter([
  {
    path: "/",
    element: <App/>,
    children: [
      {
        path: "/",
        element: <Home/>,
      },
      {
        path: "/about",
        element: <About/>,
      },
      {
        path: "/sing-in",
        element: <Auth/>,
      },
    ],
  },

]);


ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
     <RouterProvider router={router} />
  </React.StrictMode>,
)
