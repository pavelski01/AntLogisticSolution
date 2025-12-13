import { useEffect, useState } from "react";

interface Props {
  children?: React.ReactNode;
}

export default function AuthGate({ children }: Props) {
  const [ready, setReady] = useState(false);
  const [loggedIn, setLoggedIn] = useState<boolean>(false);

  useEffect(() => {
    const check = () => {
      try {
        const v = localStorage.getItem("als:isLoggedIn");
        setLoggedIn(v === "true");
      } catch {
        setLoggedIn(false);
      } finally {
        setReady(true);
      }
    };

    check();

    const onStorage = (e: StorageEvent) => {
      if (e.key === "als:isLoggedIn") {
        setLoggedIn(e.newValue === "true");
      }
    };

    window.addEventListener("storage", onStorage);
    return () => window.removeEventListener("storage", onStorage);
  }, []);

  if (!ready) return null;

  if (!loggedIn) {
    return (
      <div className="mx-auto max-w-2xl rounded-xl border border-yellow-800 bg-yellow-900/20 p-6">
        <div role="status" aria-live="polite" className="space-y-2">
          <h2 className="text-lg font-semibold text-yellow-200">Authentication Required</h2>
          <p className="text-sm text-yellow-200/90">
            Please log in to access this content.
          </p>
          <div className="pt-2">
            <a
              href="/login"
              className="inline-flex items-center rounded-lg bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 text-sm font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-purple-400"
            >
              Go to Login
            </a>
          </div>
        </div>
      </div>
    );
  }

  return <>{children}</>;
}
