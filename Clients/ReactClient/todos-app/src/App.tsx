import { useState } from 'react'
import './App.css'
import TopHeader from './components/topmenu'
import { Outlet } from "react-router-dom";


function App() {

  const [count, setCount] = useState(0)

  return (
    <>
      <TopHeader />
      <Outlet />
    </>
  )
}

export default App
