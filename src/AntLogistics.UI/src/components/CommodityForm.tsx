import { useState } from "react";

interface CreateCommodityRequest {
  sku: string;
  name: string;
  unitOfMeasure: string;
  controlParameters?: string;
  isActive?: boolean;
}

export default function CommodityForm() {
  const [form, setForm] = useState<CreateCommodityRequest>({
    sku: "",
    name: "",
    unitOfMeasure: "",
    controlParameters: "",
    isActive: true,
  });

  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const onChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value, type, checked } = e.target as HTMLInputElement;
    if (type === "checkbox") {
      setForm((f) => ({ ...f, [name]: checked }));
    } else {
      setForm((f) => ({ ...f, [name]: value }));
    }
  };

  const validate = (): string | null => {
    if (!form.sku.trim()) return "SKU is required";
    if (!form.name.trim()) return "Name is required";
    if (!form.unitOfMeasure.trim()) return "Unit of measure is required";
    const cp = form.controlParameters?.trim();
    if (cp && cp.length > 0) {
      try {
        JSON.parse(cp);
      } catch {
        return "Control parameters must be valid JSON";
      }
    }
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
      const normalizedControlParams = (() => {
        const raw = form.controlParameters?.trim() ?? "";
        if (!raw) return "{}";
        try {
          const parsed = JSON.parse(raw);
          return JSON.stringify(parsed);
        } catch {
          return raw; // backend will default if empty; validate already ran
        }
      })();

      const payload = {
        sku: form.sku.trim().toLowerCase(),
        name: form.name.trim(),
        unitOfMeasure: form.unitOfMeasure.trim(),
        controlParameters: normalizedControlParams,
        isActive: form.isActive ?? true,
      };

      const res = await fetch("/api/v1/commodities", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        const text = await res.text();
        throw new Error(`Failed to create commodity (status ${res.status}). ${text}`);
      }

      setSuccess("Commodity created successfully");
      setTimeout(() => {
        window.location.assign("/commodities");
      }, 800);
    } catch (err) {
      const msg = err instanceof Error ? err.message : "Unknown error";
      setError(msg);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <form onSubmit={onSubmit} aria-describedby="commodity-form-help" className="space-y-6">
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

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label htmlFor="sku" className="block text-sm font-medium text-gray-200">
            SKU *
          </label>
          <input
            id="sku"
            name="sku"
            required
            value={form.sku}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
        <div>
          <label htmlFor="name" className="block text-sm font-medium text-gray-200">
            Name *
          </label>
          <input
            id="name"
            name="name"
            required
            value={form.name}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
        <div className="md:col-span-2">
          <label htmlFor="unitOfMeasure" className="block text-sm font-medium text-gray-200">
            Unit of Measure *
          </label>
          <input
            id="unitOfMeasure"
            name="unitOfMeasure"
            required
            value={form.unitOfMeasure}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
        <div className="md:col-span-2">
          <label htmlFor="controlParameters" className="block text-sm font-medium text-gray-200">
            Control Parameters (JSON)
          </label>
          <textarea
            id="controlParameters"
            name="controlParameters"
            rows={5}
            placeholder={`{\n  "temperature": { "min": 2, "max": 8 }\n}`}
            value={form.controlParameters ?? ""}
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
          <label htmlFor="isActive" className="text-sm text-gray-200">
            Active
          </label>
        </div>
      </div>

      <div id="commodity-form-help" className="text-xs text-gray-400">
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
        <a href="/commodities" className="text-sm text-gray-300 hover:text-gray-200">
          Cancel
        </a>
      </div>
    </form>
  );
}
