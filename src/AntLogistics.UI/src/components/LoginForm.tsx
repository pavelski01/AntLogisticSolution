import { useState } from "react";

interface LoginRequest {
  username: string;
  password: string;
}

export default function LoginForm() {
  const [form, setForm] = useState<LoginRequest>({ username: "", password: "" });
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setForm((f) => ({ ...f, [name]: value }));
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    const username = form.username.trim();
    const password = form.password;

    if (!username || !password) {
      setError("Username and password are required");
      return;
    }

    setSubmitting(true);
    try {
      const res = await fetch("/api/v1/auth/login", {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
      });

      if (!res.ok) {
        const text = await res.text();
        throw new Error(`Login failed (status ${res.status}). ${text}`);
      }

      const data: { success?: boolean } = await res.json();
      if (data.success) {
        setSuccess("Login successful");
        setTimeout(() => {
          window.location.assign("/");
        }, 500);
      } else {
        setError("Invalid username or password");
      }
    } catch (err) {
      const msg = err instanceof Error ? err.message : "Unknown error";
      setError(msg);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <form onSubmit={onSubmit} className="space-y-6 max-w-md" aria-describedby="login-form-help">
      {error && (
        <div
          role="alert"
          className="rounded-md bg-red-900/40 border border-red-700 p-3 text-red-200"
        >
          {error}
        </div>
      )}
      {success && (
        <div
          role="status"
          className="rounded-md bg-green-900/30 border border-green-700 p-3 text-green-200"
        >
          {success}
        </div>
      )}

      <div className="space-y-4">
        <div>
          <label htmlFor="username" className="block text-sm font-medium text-gray-200">
            Username
          </label>
          <input
            id="username"
            name="username"
            autoComplete="username"
            value={form.username}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
        <div>
          <label htmlFor="password" className="block text-sm font-medium text-gray-200">
            Password
          </label>
          <input
            id="password"
            name="password"
            type="password"
            autoComplete="current-password"
            value={form.password}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
      </div>

      <div id="login-form-help" className="text-xs text-gray-400">
        Enter your operator credentials to continue.
      </div>

      <div className="flex items-center gap-3">
        <button
          type="submit"
          disabled={submitting}
          className="inline-flex items-center rounded-lg bg-purple-600 hover:bg-purple-700 disabled:opacity-60 disabled:cursor-not-allowed text-white px-4 py-2 font-medium transition-colors"
        >
          {submitting ? "Signing in..." : "Sign in"}
        </button>
        <a href="/" className="text-sm text-gray-300 hover:text-gray-200">
          Cancel
        </a>
      </div>
    </form>
  );
}
