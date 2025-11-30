import { useState, useEffect } from "react";

interface Warehouse {
  id: number;
  code: string;
  name: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
  capacity: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
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
      setError(null);
      
      const response = await fetch('/api/v1/warehouses?includeInactive=false');
      
      if (!response.ok) {
        throw new Error(`API returned status ${response.status}`);
      }
      
      const data = await response.json();
      setWarehouses(data);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to load warehouses';
      setError(errorMessage);
      console.error('Error fetching warehouses:', err);
    } finally {
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
    return (
      <div className="text-center mt-8">
        <div className="text-red-400 text-xl mb-4">{error}</div>
        <button
          onClick={fetchWarehouses}
          className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-lg transition-colors"
        >
          Retry
        </button>
      </div>
    );
  }

  if (warehouses.length === 0) {
    return (
      <div className="text-center text-gray-400 text-xl mt-8">
        No warehouses found. Create one to get started!
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mt-8">
      {warehouses.map((warehouse) => (
        <div
          key={warehouse.id}
          className="bg-gray-800 p-6 rounded-lg shadow-md hover:-translate-y-1 hover:shadow-xl transition-all duration-300"
        >
          <div className="flex justify-between items-start mb-3">
            <h3 className="text-xl font-bold text-purple-400">{warehouse.name}</h3>
            <span className="text-xs bg-purple-600 text-white px-2 py-1 rounded">
              {warehouse.code}
            </span>
          </div>
          <p className="text-gray-300 mb-2">
            <strong>Address:</strong> {warehouse.address}
          </p>
          <p className="text-gray-300 mb-2">
            <strong>City:</strong> {warehouse.city}, {warehouse.state} {warehouse.zipCode}
          </p>
          <p className="text-gray-300 mb-2">
            <strong>Country:</strong> {warehouse.country}
          </p>
          <p className="text-gray-300">
            <strong>Capacity:</strong> {warehouse.capacity.toLocaleString()} units
          </p>
        </div>
      ))}
    </div>
  );
}
