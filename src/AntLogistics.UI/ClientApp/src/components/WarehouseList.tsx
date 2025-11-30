import { useState, useEffect } from "react";

interface Warehouse {
  id: number;
  name: string;
  location: string;
  capacity: number;
}

export default function WarehouseList() {
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchWarehouses();
  }, []);

  const fetchWarehouses = async () => {
    try {
      setLoading(true);
      // TODO: Replace with actual API endpoint
      // const response = await fetch('/api/warehouses')
      // const data = await response.json()
      // setWarehouses(data)

      // Mock data for now
      setTimeout(() => {
        setWarehouses([
          { id: 1, name: "Warehouse A", location: "New York", capacity: 1000 },
          { id: 2, name: "Warehouse B", location: "Los Angeles", capacity: 1500 },
          { id: 3, name: "Warehouse C", location: "Chicago", capacity: 800 },
        ]);
        setLoading(false);
      }, 500);
    } catch (err) {
      setError("Failed to load warehouses");
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="text-center text-xl mt-8">
        <div className="animate-pulse">Loading warehouses...</div>
      </div>
    );
  }

  if (error) {
    return <div className="text-center text-red-400 text-xl mt-8">{error}</div>;
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mt-8">
      {warehouses.map((warehouse) => (
        <div
          key={warehouse.id}
          className="bg-gray-800 p-6 rounded-lg shadow-md hover:-translate-y-1 hover:shadow-xl transition-all duration-300"
        >
          <h3 className="text-xl font-bold text-purple-400 mb-3">{warehouse.name}</h3>
          <p className="text-gray-300 mb-2">
            <strong>Location:</strong> {warehouse.location}
          </p>
          <p className="text-gray-300">
            <strong>Capacity:</strong> {warehouse.capacity} units
          </p>
        </div>
      ))}
    </div>
  );
}
