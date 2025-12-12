import { useState } from "react";

interface CreateWarehouseRequest {
  name: string;
  code: string;
  addressLine: string;
  city: string;
  countryCode: string;
  postalCode?: string | null;
  defaultZone?: string;
  capacity?: number;
  isActive?: boolean;
}

export default function WarehouseForm() {
  const [form, setForm] = useState<CreateWarehouseRequest>({
    name: "",
    code: "",
    addressLine: "",
    city: "",
    countryCode: "",
    postalCode: "",
    defaultZone: "DEFAULT",
    capacity: 0,
    isActive: true,
  });

  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const onChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value, type, checked } = e.target as HTMLInputElement;
    if (type === "checkbox") {
      setForm((f) => ({ ...f, [name]: checked }));
    } else if (name === "capacity") {
      const n = value === "" ? 0 : Number(value);
      setForm((f) => ({ ...f, capacity: isNaN(n) ? 0 : n }));
    } else if (name === "countryCode") {
      setForm((f) => ({ ...f, countryCode: value.toUpperCase() }));
    } else if (name === "postalCode") {
      setForm((f) => ({ ...f, postalCode: value === "" ? null : value }));
    } else {
      setForm((f) => ({ ...f, [name]: value }));
    }
  };

  const validate = (): string | null => {
    if (!form.name.trim()) return "Name is required";
    if (!form.code.trim()) return "Code is required";
    if (!form.addressLine.trim()) return "Address line is required";
    if (!form.city.trim()) return "City is required";
    if (!form.countryCode.trim()) return "Country code is required";
    if (form.countryCode.length !== 2)
      return "Country code must be 2 letters (ISO-3166-1)";
    if ((form.capacity ?? 0) < 0) return "Capacity cannot be negative";
    return null;
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);
    const errorMsg = validate();
    if (errorMsg) {
      setError(errorMsg);
      return;
    }

    setSubmitting(true);
    try {
      const payload = {
        // send camelCase keys (ASP.NET binding is case-insensitive)
        name: form.name.trim(),
        code: form.code.trim(),
        addressLine: form.addressLine.trim(),
        city: form.city.trim(),
        countryCode: form.countryCode.trim().toUpperCase(),
        postalCode: form.postalCode ?? null,
        defaultZone: form.defaultZone ?? "DEFAULT",
        capacity: form.capacity ?? 0,
        isActive: form.isActive ?? true,
      };

      const res = await fetch("/api/v1/warehouses", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        const text = await res.text();
        throw new Error(
          `Failed to create warehouse (status ${res.status}). ${text}`
        );
      }

      setSuccess("Warehouse created successfully");
      // Redirect to list after a small delay
      setTimeout(() => {
        window.location.assign("/warehouses");
      }, 800);
    } catch (err) {
      const msg = err instanceof Error ? err.message : "Unknown error";
      setError(msg);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <form onSubmit={onSubmit} aria-describedby="form-help" className="space-y-6">
      {error && (
        <div role="alert" className="rounded-md bg-red-900/40 border border-red-700 p-3 text-red-200">
          {error}
        </div>
      )}
      {success && (
        <div role="status" className="rounded-md bg-green-900/30 border border-green-700 p-3 text-green-200">
          {success}
        </div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label htmlFor="name" className="block text-sm font-medium text-gray-200">Name *</label>
          <input
            id="name"
            name="name"
            required
            value={form.name}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
        <div>
          <label htmlFor="code" className="block text-sm font-medium text-gray-200">Code *</label>
          <input
            id="code"
            name="code"
            required
            value={form.code}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
        <div className="md:col-span-2">
          <label htmlFor="addressLine" className="block text-sm font-medium text-gray-200">Address Line *</label>
          <input
            id="addressLine"
            name="addressLine"
            required
            value={form.addressLine}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
        <div>
          <label htmlFor="city" className="block text-sm font-medium text-gray-200">City *</label>
          <input
            id="city"
            name="city"
            required
            value={form.city}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
        <div>
          <label htmlFor="countryCode" className="block text-sm font-medium text-gray-200">Country Code (ISO-2) *</label>
          <input
            id="countryCode"
            name="countryCode"
            required
            maxLength={2}
            value={form.countryCode}
            onChange={onChange}
            className="mt-1 uppercase tracking-wider w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
        <div>
          <label htmlFor="postalCode" className="block text-sm font-medium text-gray-200">Postal Code</label>
          <input
            id="postalCode"
            name="postalCode"
            value={form.postalCode ?? ""}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
        <div>
          <label htmlFor="defaultZone" className="block text-sm font-medium text-gray-200">Default Zone</label>
          <input
            id="defaultZone"
            name="defaultZone"
            value={form.defaultZone ?? ""}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
        <div>
          <label htmlFor="capacity" className="block text-sm font-medium text-gray-200">Capacity</label>
          <input
            id="capacity"
            name="capacity"
            type="number"
            min={0}
            step="0.01"
            value={form.capacity ?? 0}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
        <div className="flex items-center gap-3 md:col-span-2">
          <input
            id="isActive"
            name="isActive"
            type="checkbox"
            checked={form.isActive ?? true}
            onChange={onChange}
            className="h-4 w-4 rounded border-gray-700 bg-gray-900 text-purple-600 focus:ring-purple-600"
          />
          <label htmlFor="isActive" className="text-sm text-gray-200">Active</label>
        </div>
      </div>

      <div id="form-help" className="text-xs text-gray-400">
        Fields marked * are required.
      </div>

      <div className="flex items-center gap-3">
        <button
          type="submit"
          disabled={submitting}
          className="inline-flex items-center rounded-lg bg-purple-600 hover:bg-purple-700 disabled:opacity-60 disabled:cursor-not-allowed text-white px-4 py-2 font-medium transition-colors"
        >
          {submitting ? "Creating..." : "Create"}
        </button>
        <a href="/warehouses" className="text-sm text-gray-300 hover:text-gray-200">Cancel</a>
      </div>
    </form>
  );
}
