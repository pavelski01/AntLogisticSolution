/// <reference types="vitest" />
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import LoginForm from "../LoginForm";
import { beforeEach, describe, expect, it, vi } from "vitest";

describe("LoginForm", () => {
  beforeEach(() => {
    vi.restoreAllMocks();
  });

  it("validates required fields", async () => {
    render(<LoginForm />);

    await userEvent.click(screen.getByRole("button", { name: /sign in/i }));

    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Username and password are required"
    );
  });

  it("shows success on valid login", async () => {
    vi.spyOn(global, "fetch" as any).mockResolvedValueOnce(
      new Response(JSON.stringify({ success: true }), {
        status: 200,
        headers: { "Content-Type": "application/json" },
      })
    );

    render(<LoginForm />);

    await userEvent.type(screen.getByLabelText(/username/i), "operator");
    await userEvent.type(screen.getByLabelText(/password/i), "secret");

    await userEvent.click(screen.getByRole("button", { name: /sign in/i }));

    expect(await screen.findByRole("status")).toHaveTextContent("Login successful");
  });
});
