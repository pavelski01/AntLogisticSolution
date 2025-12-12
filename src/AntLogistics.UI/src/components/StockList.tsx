import { useState, useEffect } from "react";

interface Stock {
  id: number;
  warehouseId: string;
  commodityId: string;
  sku: string;
  unitOfMeasure: string;
  quantity: number;
  warehouseZone: string;
  operatorId: string | null;
  createdBy: string;
  source: string;
  occurredAt: string;
  createdAt: string;
  metadata: string;
}

interface Warehouse {
  id: string;
  code: string;
  name: string;
}

interface Commodity {
  id: string;
  sku: string;
  name: string;
}

export default function StockList() {
  const [stocks, setStocks] = useState<Stock[]>([]);
  const [warehouses, setWarehouses] = useState<Map<string, Warehouse>>(new Map());
  const [commodities, setCommodities] = useState<Map<string, Commodity>>(new Map());
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);
      
      const [stocksResponse, warehousesResponse, commoditiesResponse] = await Promise.all([
        fetch('/api/v1/stocks'),
        fetch('/api/v1/warehouses?includeInactive=false'),
        fetch('/api/v1/commodities?includeInactive=false')
      ]);
      
      if (!stocksResponse.ok || !warehousesResponse.ok || !commoditiesResponse.ok) {
        throw new Error('Failed to fetch data');
      }
      
      const stocksData = await stocksResponse.json();
      const warehousesData = await warehousesResponse.json();
      const commoditiesData = await commoditiesResponse.json();
      
      const warehouseMap = new Map<string, Warehouse>(
        warehousesData.map((w: Warehouse) => [w.id, w])
      );
      
      const commodityMap = new Map<string, Commodity>(
        commoditiesData.map((c: Commodity) => [c.id, c])
      );
      
      setStocks(stocksData);
      setWarehouses(warehouseMap);
      setCommodities(commodityMap);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to load stocks';
      setError(errorMessage);
      console.error('Error fetching stocks:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchStocks = fetchData;

  if (loading) {
    return (
      <div className="text-center text-xl mt-8">
        <div className="animate-pulse">Loading stocks...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center mt-8">
        <div className="text-red-400 text-xl mb-4">{error}</div>
        <button
          onClick={fetchStocks}
          className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-lg transition-colors"
        >
          Retry
        </button>
      </div>
    );
  }

  if (stocks.length === 0) {
    return (
      <div className="text-center text-gray-400 text-xl mt-8">
        No stock records found. Create one to get started!
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mt-8">
      {stocks.map((stock) => {
        const warehouse = warehouses.get(stock.warehouseId);
        const commodity = commodities.get(stock.commodityId);
        return (
        <div
          key={stock.id}
          className="bg-gray-800 p-6 rounded-lg shadow-md hover:-translate-y-1 hover:shadow-xl transition-all duration-300"
        >
          <div className="flex justify-between items-start mb-3">
            <h3 className="text-xl font-bold text-purple-400">{commodity?.name || stock.sku}</h3>
            <span className="text-xs bg-purple-600 text-white px-2 py-1 rounded">
              {stock.source}
            </span>
          </div>
          <p className="text-gray-300 mb-2">
            <strong>SKU:</strong> {stock.sku}
          </p>
          <p className="text-gray-300 mb-2">
            <strong>Warehouse:</strong> {warehouse?.name || stock.warehouseId}
          </p>
          <p className="text-gray-300 mb-2">
            <strong>Unit:</strong> {stock.unitOfMeasure}
          </p>
          <div className="border-t border-gray-700 pt-3 mt-3">
            <p className="text-gray-300 mb-1">
              <strong>Quantity:</strong>
            </p>
            <p className="text-green-400 font-semibold text-2xl mb-2">
              {stock.quantity.toLocaleString()} {stock.unitOfMeasure}
            </p>
          </div>
          <div className="border-t border-gray-700 pt-3 mt-3">
            <p className="text-gray-400 text-sm mb-1">
              <strong>Created by:</strong> {stock.createdBy}
            </p>
            <p className="text-gray-400 text-sm mb-1">
              <strong>Occurred:</strong> {new Date(stock.occurredAt).toLocaleString()}
            </p>
            <p className="text-gray-400 text-sm">
              <strong>Recorded:</strong> {new Date(stock.createdAt).toLocaleString()}
            </p>
          </div>
        </div>
      );
      })}
    </div>
  );
}
