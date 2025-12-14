import { useEffect, useState } from "react";

interface Warehouse {
  id: string;
  code: string;
  name: string;
}

interface Commodity {
  id: string;
  sku: string;
  name: string;
}

interface CreateStockForm {
  warehouseId: string;
  commodityId: string;
  quantity: string; // keep as string for controlled input, convert on submit
  warehouseZone?: string;
  operatorId?: string;
  createdBy?: string;
  source?: string;
  occurredAt?: string; // datetime-local
  metadata?: string; // JSON string
}

export default function StockForm() {
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [commodities, setCommodities] = useState<Commodity[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [success, setSuccess] = useState<string | null>(null);

  const [form, setForm] = useState<CreateStockForm>({
    warehouseId: "",
    commodityId: "",
    quantity: "",
    warehouseZone: "",
    operatorId: "",
    createdBy: "",
    source: "manual",
    occurredAt: "",
    metadata: "{}",
  });

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        setError(null);
        const [wRes, cRes] = await Promise.all([
          fetch("/api/v1/warehouses?includeInactive=false"),
          fetch("/api/v1/commodities?includeInactive=false"),
        ]);
        if (!wRes.ok || !cRes.ok) throw new Error("Failed to load lookups");
        const [wData, cData] = await Promise.all([wRes.json(), cRes.json()]);
        setWarehouses(wData);
        setCommodities(cData);
      } catch (e) {
        const msg = e instanceof Error ? e.message : "Failed to load data";
        setError(msg);
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const onChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setForm((f) => ({ ...f, [name]: value }));
  };

  const isGuid = (v: string) =>
    /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$/.test(
      v.trim()
    );

  const validate = (): string | null => {
    if (!form.warehouseId) return "Warehouse is required";
    if (!form.commodityId) return "Commodity is required";
    if (!form.quantity.trim()) return "Quantity is required";
    const q = Number(form.quantity);
    if (!Number.isFinite(q)) return "Quantity must be a number";
    if (q <= 0) return "Quantity must be greater than zero";
    const op = form.operatorId?.trim();
    if (op && !isGuid(op)) return "OperatorId must be a valid GUID";
    const md = form.metadata?.trim();
    if (md) {
      try {
        JSON.parse(md);
      } catch {
        return "Metadata must be valid JSON";
      }
    }
    return null;
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);
    const err = validate();
    if (err) {
      setError(err);
      return;
    }
    setSubmitting(true);
    try {
      const q = Number(form.quantity);
      const occurredAtIso = form.occurredAt?.trim()
        ? new Date(form.occurredAt).toISOString()
        : undefined;
      const payload: Record<string, unknown> = {
        warehouseId: form.warehouseId,
        commodityId: form.commodityId,
        quantity: q,
      };
      if (form.warehouseZone?.trim()) payload.warehouseZone = form.warehouseZone.trim();
      if (form.operatorId?.trim()) payload.operatorId = form.operatorId.trim();
      if (form.createdBy?.trim()) payload.createdBy = form.createdBy.trim();
      if (form.source?.trim()) payload.source = form.source.trim();
      if (occurredAtIso) payload.occurredAt = occurredAtIso;
      if (form.metadata?.trim()) payload.metadata = form.metadata.trim();

      const res = await fetch("/api/v1/stocks", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });
      if (!res.ok) {
        const text = await res.text();
        throw new Error(`Failed to create stock (status ${res.status}). ${text}`);
      }
      setSuccess("Stock record created successfully");
      setTimeout(() => {
        window.location.assign("/stocks");
      }, 800);
    } catch (e) {
      const msg = e instanceof Error ? e.message : "Unknown error";
      setError(msg);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="text-center text-xl mt-8">
        <div className="animate-pulse">Loading form...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center mt-8">
        <div className="text-red-400 text-xl mb-4">{error}</div>
        <button
          onClick={() => window.location.reload()}
          className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-lg transition-colors"
        >
          Retry
        </button>
      </div>
    );
  }

  return (
    <form onSubmit={onSubmit} aria-describedby="stock-form-help" className="space-y-6">
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
          <label htmlFor="warehouseId" className="block text-sm font-medium text-gray-200">
            Warehouse *
          </label>
          <select
            id="warehouseId"
            name="warehouseId"
            required
            value={form.warehouseId}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          >
            <option value="" disabled>
              Select warehouse
            </option>
            {warehouses.map((w) => (
              <option key={w.id} value={w.id}>
                {w.name} ({w.code})
              </option>
            ))}
          </select>
        </div>

        <div>
          <label htmlFor="commodityId" className="block text-sm font-medium text-gray-200">
            Commodity *
          </label>
          <select
            id="commodityId"
            name="commodityId"
            required
            value={form.commodityId}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          >
            <option value="" disabled>
              Select commodity
            </option>
            {commodities.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name} ({c.sku})
              </option>
            ))}
          </select>
        </div>

        <div>
          <label htmlFor="quantity" className="block text-sm font-medium text-gray-200">
            Quantity *
          </label>
          <input
            id="quantity"
            name="quantity"
            required
            inputMode="decimal"
            value={form.quantity}
            onChange={onChange}
            placeholder="e.g., 100.5"
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>

        <div>
          <label htmlFor="warehouseZone" className="block text-sm font-medium text-gray-200">
            Warehouse Zone
          </label>
          <input
            id="warehouseZone"
            name="warehouseZone"
            value={form.warehouseZone || ""}
            onChange={onChange}
            placeholder="e.g., A-12"
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>

        <div>
          <label htmlFor="operatorId" className="block text-sm font-medium text-gray-200">
            OperatorId (GUID)
          </label>
          <input
            id="operatorId"
            name="operatorId"
            value={form.operatorId || ""}
            onChange={onChange}
            placeholder="00000000-0000-0000-0000-000000000000"
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>

        <div>
          <label htmlFor="createdBy" className="block text-sm font-medium text-gray-200">
            Created By
          </label>
          <input
            id="createdBy"
            name="createdBy"
            value={form.createdBy || ""}
            onChange={onChange}
            placeholder="Operator name"
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>

        <div>
          <label htmlFor="source" className="block text-sm font-medium text-gray-200">
            Source
          </label>
          <input
            id="source"
            name="source"
            value={form.source || ""}
            onChange={onChange}
            placeholder="manual"
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>

        <div>
          <label htmlFor="occurredAt" className="block text-sm font-medium text-gray-200">
            Occurred At
          </label>
          <input
            id="occurredAt"
            name="occurredAt"
            type="datetime-local"
            value={form.occurredAt || ""}
            onChange={onChange}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>

        <div className="md:col-span-2">
          <label htmlFor="metadata" className="block text-sm font-medium text-gray-200">
            Metadata (JSON)
          </label>
          <textarea
            id="metadata"
            name="metadata"
            rows={4}
            value={form.metadata || ""}
            onChange={onChange}
            placeholder={`{
  "batch": "B-2025-12",
  "notes": "optional"
}`}
            className="mt-1 w-full rounded-md bg-gray-900 border border-gray-700 px-3 py-2 text-gray-100 focus:outline-none focus:ring-2 focus:ring-purple-600"
          />
        </div>
      </div>

      <div id="stock-form-help" className="text-xs text-gray-400">
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
        <a href="/stocks" className="text-sm text-gray-300 hover:text-gray-200">
          Cancel
        </a>
      </div>
    </form>
  );
}
