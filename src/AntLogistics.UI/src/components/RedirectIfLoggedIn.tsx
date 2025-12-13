import { useEffect } from "react";

export default function RedirectIfLoggedIn() {
  useEffect(() => {
    const run = async () => {
      try {
        const res = await fetch("/api/v1/auth/me", { credentials: "include" });
        if (res.ok) {
          window.location.replace("/");
        }
      } catch {
        // ignore
      }
    };
    run();
  }, []);

  return null;
}
