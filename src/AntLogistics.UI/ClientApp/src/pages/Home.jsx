import { useState, useEffect } from 'react'
import './pages.css'

function Home() {
  return (
    <div className="home-page">
      <h1>Welcome to Ant Logistics</h1>
      <p>Modern logistics management powered by .NET Aspire and React</p>
      
      <div className="features">
        <div className="feature-card">
          <h3>📦 Warehouse Management</h3>
          <p>Track and manage your warehouses efficiently</p>
        </div>
        <div className="feature-card">
          <h3>🚚 Commodity Tracking</h3>
          <p>Monitor commodities across all locations</p>
        </div>
        <div className="feature-card">
          <h3>📊 Analytics</h3>
          <p>Get insights with real-time analytics</p>
        </div>
      </div>
    </div>
  )
}

export default Home
