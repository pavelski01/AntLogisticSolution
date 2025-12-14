/// <reference types="vitest" />
import { render, screen, waitFor } from '@testing-library/react';
import AuthGate from '../AuthGate';
import { beforeEach, describe, expect, it, vi } from 'vitest';

describe('AuthGate', () => {
  beforeEach(() => {
    vi.restoreAllMocks();
  });

  it('renders login prompt when not authenticated', async () => {
    vi.spyOn(global, 'fetch' as any).mockResolvedValueOnce(
      new Response('', { status: 401 })
    );

    render(
      <AuthGate>
        <div>Protected</div>
      </AuthGate>
    );

    await waitFor(() => {
      expect(screen.getByText('Authentication Required')).toBeInTheDocument();
    });
    expect(screen.queryByText('Protected')).not.toBeInTheDocument();
  });

  it('renders children when authenticated', async () => {
    vi.spyOn(global, 'fetch' as any).mockResolvedValueOnce(
      new Response('', { status: 200 })
    );

    render(
      <AuthGate>
        <div>Protected</div>
      </AuthGate>
    );

    await waitFor(() => {
      expect(screen.getByText('Protected')).toBeInTheDocument();
    });
    expect(
      screen.queryByText('Authentication Required')
    ).not.toBeInTheDocument();
  });
});
