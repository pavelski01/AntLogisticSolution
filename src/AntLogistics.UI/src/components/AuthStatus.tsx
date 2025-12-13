import { useEffect, useState } from "react";

export default function AuthStatus() {
  const [isLoggedIn, setIsLoggedIn] = useState<boolean>(false);
  const [username, setUsername] = useState<string | null>(null);

  useEffect(() => {
    const read = () => {
      try {
        const logged = localStorage.getItem("als:isLoggedIn") === "true";
        const name = localStorage.getItem("als:username") || null;
        setIsLoggedIn(logged);
        setUsername(name);
      } catch {
        setIsLoggedIn(false);
        setUsername(null);
      }
    };

    read();

    const onStorage = (e: StorageEvent) => {
      if (e.key === "als:isLoggedIn" || e.key === "als:username") {
        read();
      }
    };

    window.addEventListener("storage", onStorage);
    return () => window.removeEventListener("storage", onStorage);
  }, []);

  const onLogout = () => {
    try {
      localStorage.removeItem("als:isLoggedIn");
      localStorage.removeItem("als:username");
    } catch {}
    setIsLoggedIn(false);
    setUsername(null);
    window.location.assign("/login");
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
