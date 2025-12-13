import { useEffect } from "react";

export default function RedirectIfLoggedIn() {
  useEffect(() => {
    try {
      const isLoggedIn = localStorage.getItem("als:isLoggedIn") === "true";
      if (isLoggedIn) {
        window.location.replace("/");
      }
    } catch {
      // ignore
    }
  }, []);

  return null;
}
