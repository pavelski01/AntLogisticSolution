import { useEffect, useState } from "react";

export default function AuthStatus() {
  const [isLoggedIn, setIsLoggedIn] = useState<boolean>(false);
  const [username, setUsername] = useState<string | null>(null);

  useEffect(() => {
    const fetchMe = async () => {
      try {
        const res = await fetch("/api/v1/auth/me", { credentials: "include" });
        if (!res.ok) {
          setIsLoggedIn(false);
          setUsername(null);
          return;
        }
        const data: { username: string } = await res.json();
        setIsLoggedIn(true);
        setUsername(data.username);
      } catch {
        setIsLoggedIn(false);
        setUsername(null);
      }
    };
    fetchMe();
  }, []);

  const onLogout = async () => {
    try {
      await fetch("/api/v1/auth/logout", { method: "POST", credentials: "include" });
    } finally {
      window.location.assign("/login");
    }
  };

  if (!isLoggedIn) {
    return (
      <a
        href="/login"
        className="inline-flex items-center rounded-lg bg-white/10 hover:bg-white/20 text-white px-3 py-2 text-sm font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-white/40"
      >
        Login
      </a>
    );
  }

  return (
    <button
      type="button"
      onClick={onLogout}
      className="inline-flex items-center gap-2 rounded-lg bg-white/10 hover:bg-white/20 text-white px-3 py-2 text-sm font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-white/40"
      aria-label="Logout"
      title="Logout"
    >
      <span className="inline-block h-2 w-2 rounded-full bg-green-500" aria-hidden="true" />
      <span>{username ?? "User"}</span>
      <span className="opacity-80">· Logout</span>
    </button>
  );
}
