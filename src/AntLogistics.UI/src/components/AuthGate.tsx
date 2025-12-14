import { useEffect, useState } from "react";

interface Props {
  children?: React.ReactNode;
}

export default function AuthGate({ children }: Props) {
  const [ready, setReady] = useState(false);
  const [loggedIn, setLoggedIn] = useState<boolean>(false);

  useEffect(() => {
    const check = async () => {
      try {
        const res = await fetch("/api/v1/auth/me", { credentials: "include" });
        setLoggedIn(res.ok);
      } catch {
        setLoggedIn(false);
      } finally {
        setReady(true);
      }
    };
    check();
  }, []);

  if (!ready) return null;

  if (!loggedIn) {
    return (
      <div className="mx-auto max-w-4xl rounded-xl border border-yellow-800 bg-yellow-900/20 p-6">
        <div className="flex items-center gap-8">
          <img
            src="/images/logo.png"
            alt=""
            aria-hidden="true"
            className="w-40 h-40 sm:w-48 sm:h-48 md:w-56 md:h-56 rounded-lg shadow-lg"
          />
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
      </div>
    );
  }

  return <>{children}</>;
}
