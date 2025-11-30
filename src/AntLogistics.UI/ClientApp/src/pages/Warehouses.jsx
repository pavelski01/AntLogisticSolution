import { useState, useEffect } from 'react'
import './pages.css'

function Warehouses() {
  const [warehouses, setWarehouses] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    fetchWarehouses()
  }, [])

  const fetchWarehouses = async () => {
    try {
      setLoading(true)
      // TODO: Replace with actual API endpoint
      // const response = await fetch('/api/warehouses')
      // const data = await response.json()
      // setWarehouses(data)
      
      // Mock data for now
      setTimeout(() => {
        setWarehouses([
          { id: 1, name: 'Warehouse A', location: 'New York', capacity: 1000 },
          { id: 2, name: 'Warehouse B', location: 'Los Angeles', capacity: 1500 },
          { id: 3, name: 'Warehouse C', location: 'Chicago', capacity: 800 },
        ])
        setLoading(false)
      }, 500)
    } catch (err) {
      setError('Failed to load warehouses')
      setLoading(false)
    }
  }

  if (loading) return <div className="loading">Loading warehouses...</div>
  if (error) return <div className="error">{error}</div>

  return (
    <div className="warehouses-page">
      <h1>Warehouses</h1>
      <div className="warehouses-grid">
        {warehouses.map(warehouse => (
          <div key={warehouse.id} className="warehouse-card">
            <h3>{warehouse.name}</h3>
            <p><strong>Location:</strong> {warehouse.location}</p>
            <p><strong>Capacity:</strong> {warehouse.capacity} units</p>
          </div>
        ))}
      </div>
    </div>
  )
}

export default Warehouses
