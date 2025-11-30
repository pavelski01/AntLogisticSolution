import { useState } from 'react'
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom'
import './App.css'
import Home from './pages/Home'
import Warehouses from './pages/Warehouses'

function App() {
  return (
    <Router>
      <div className="App">
        <nav className="navbar">
          <div className="nav-brand">
            <h1>🐜 Ant Logistics</h1>
          </div>
          <ul className="nav-links">
            <li><Link to="/">Home</Link></li>
            <li><Link to="/warehouses">Warehouses</Link></li>
          </ul>
        </nav>

        <main className="main-content">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/warehouses" element={<Warehouses />} />
          </Routes>
        </main>

        <footer className="footer">
          <p>&copy; 2025 Ant Logistics. Built with .NET Aspire + React</p>
        </footer>
      </div>
    </Router>
  )
}

export default App
