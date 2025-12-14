import { useState, useEffect } from "react";

interface Commodity {
  id: string;
  sku: string;
  name: string;
  unitOfMeasure: string;
  controlParameters: string;
  isActive: boolean;
  deactivatedAt: string | null;
  createdAt: string;
  updatedAt: string;
}

export default function CommodityList() {
  const [commodities, setCommodities] = useState<Commodity[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchCommodities();
  }, []);

  const fetchCommodities = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await fetch("/api/v1/commodities?includeInactive=false");

      if (!response.ok) {
        throw new Error(`API returned status ${response.status}`);
      }

      const data = await response.json();
      setCommodities(data);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : "Failed to load commodities";
      setError(errorMessage);
      console.error("Error fetching commodities:", err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="text-center text-xl mt-8">
        <div className="animate-pulse">Loading commodities...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center mt-8">
        <div className="text-red-400 text-xl mb-4">{error}</div>
        <button
          onClick={fetchCommodities}
          className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-lg transition-colors"
        >
          Retry
        </button>
      </div>
    );
  }

  if (commodities.length === 0) {
    return (
      <div className="text-center text-gray-400 text-xl mt-8">
        No commodities found. Create one to get started!
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mt-8">
      {commodities.map((commodity) => (
        <div
          key={commodity.id}
          className="bg-gray-800 p-6 rounded-lg shadow-md hover:-translate-y-1 hover:shadow-xl transition-all duration-300"
        >
          <div className="flex justify-between items-start mb-3">
            <h3 className="text-xl font-bold text-purple-400">{commodity.name}</h3>
            <span className="text-xs bg-purple-600 text-white px-2 py-1 rounded">
              {commodity.sku}
            </span>
          </div>
          <p className="text-gray-300 mb-2">
            <strong>Unit of Measure:</strong> {commodity.unitOfMeasure}
          </p>
          <p className="text-gray-300 mb-2">
            <strong>Status:</strong> {commodity.isActive ? "Active" : "Inactive"}
            {!commodity.isActive && commodity.deactivatedAt
              ? ` since ${new Date(commodity.deactivatedAt).toLocaleDateString()}`
              : ""}
          </p>
          <div className="text-gray-400 text-sm mt-4 pt-4 border-t border-gray-700">
            <p>Created: {new Date(commodity.createdAt).toLocaleDateString()}</p>
            <p>Updated: {new Date(commodity.updatedAt).toLocaleDateString()}</p>
          </div>
        </div>
      ))}
    </div>
  );
}
