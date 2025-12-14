/// <reference types="vitest" />
import { render, screen, waitFor } from "@testing-library/react";
import ApiStatus from "../ApiStatus";
import { beforeEach, describe, expect, it, vi } from "vitest";

describe("ApiStatus", () => {
  beforeEach(() => {
    vi.restoreAllMocks();
  });

  it("shows online when /health is ok", async () => {
    vi.spyOn(global, "fetch" as any).mockResolvedValueOnce(new Response("", { status: 200 }));

    render(<ApiStatus />);

    expect(screen.getByText("Checking API...")).toBeInTheDocument();

    await waitFor(() => {
      expect(screen.getByText("API Online")).toBeInTheDocument();
    });
  });

  it("shows offline when request fails", async () => {
    vi.spyOn(global, "fetch" as any).mockRejectedValueOnce(new Error("network"));

    render(<ApiStatus />);

    await waitFor(() => {
      expect(screen.getByText("API Offline")).toBeInTheDocument();
    });
  });
});
