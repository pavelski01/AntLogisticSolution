import { useEffect, useState } from "react";

export default function ApiStatus() {
  const [isOnline, setIsOnline] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(true);

  useEffect(() => {
    const checkApi = async () => {
      try {
        const response = await fetch("/health");

        if (response.ok) {
          setIsOnline(true);
        } else {
          setIsOnline(false);
        }
      } catch (error) {
        setIsOnline(false);
        console.error("API health check error:", error);
      } finally {
        setIsLoading(false);
      }
    };

    checkApi();
  }, []);

  return (
    <div className="flex items-center gap-2 bg-gray-800/80 rounded-lg px-3 py-2">
      <div
        className={`w-2 h-2 rounded-full ${
          isLoading ? "bg-yellow-500 animate-pulse" : isOnline ? "bg-green-500" : "bg-red-500"
        }`}
      />
      <span className="text-xs text-gray-300">
        {isLoading ? "Checking API..." : isOnline ? "API Online" : "API Offline"}
      </span>
    </div>
  );
}
