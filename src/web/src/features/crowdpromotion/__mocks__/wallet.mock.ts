import type {
    PromotorWallet,
    WalletTransaccionItem,
    WalletTransaccionesPagedResponse,
    SolicitarCobroResponse,
} from "@shared/types/crowdpromotion"

// ========== Wallet Resumen ==========

export const mockWallet: PromotorWallet = {
    walletId: "wallet-001",
    monedaId: 1,
    monedaNombre: "EUR",
    saldoDisponible: 150.50,
    saldoPendiente: 25.00,
    totalGanado: 500.00,
    totalRetirado: 324.50,
    minimoRetiro: 10.00,
}

export const mockWallet_SaldoBajo: PromotorWallet = {
    ...mockWallet,
    saldoDisponible: 5.00,
    totalGanado: 5.00,
    totalRetirado: 0,
}

export const mockWallet_SaldoCero: PromotorWallet = {
    ...mockWallet,
    saldoDisponible: 0,
    saldoPendiente: 0,
    totalGanado: 0,
    totalRetirado: 0,
}

// ========== Transacciones ==========

export const mockTransaccionCredito: WalletTransaccionItem = {
    id: "tx-001",
    esCredito: true,
    importe: 25.50,
    descripcion: "Comision por conversion",
    concepto: "Comision backing #123",
    estadoTransaccionId: 2,
    estadoTransaccionNombre: "Procesada",
    tipoRewardId: 1,
    tipoRewardNombre: "Dinero",
    promoEventoId: "evt-001",
    fechaCreacion: "2026-02-15T10:30:00Z",
    fechaProcesado: "2026-02-15T11:00:00Z",
}

export const mockTransaccionDebito: WalletTransaccionItem = {
    id: "tx-002",
    esCredito: false,
    importe: 50.00,
    descripcion: "Solicitud de cobro",
    concepto: "Retiro a cuenta bancaria",
    estadoTransaccionId: 1,
    estadoTransaccionNombre: "Pendiente",
    tipoRewardId: null,
    tipoRewardNombre: null,
    promoEventoId: null,
    fechaCreacion: "2026-02-20T14:00:00Z",
    fechaProcesado: null,
}

export const mockTransaccionPagada: WalletTransaccionItem = {
    id: "tx-003",
    esCredito: false,
    importe: 100.00,
    descripcion: null,
    concepto: "Cobro procesado",
    estadoTransaccionId: 3,
    estadoTransaccionNombre: "Pagada",
    tipoRewardId: null,
    tipoRewardNombre: null,
    promoEventoId: null,
    fechaCreacion: "2026-01-10T08:00:00Z",
    fechaProcesado: "2026-01-12T16:00:00Z",
}

export const mockTransaccionCancelada: WalletTransaccionItem = {
    id: "tx-004",
    esCredito: false,
    importe: 30.00,
    descripcion: "Solicitud cancelada",
    concepto: null,
    estadoTransaccionId: 4,
    estadoTransaccionNombre: "Cancelada",
    tipoRewardId: null,
    tipoRewardNombre: null,
    promoEventoId: null,
    fechaCreacion: "2026-01-05T12:00:00Z",
    fechaProcesado: null,
}

export const mockTransacciones: WalletTransaccionItem[] = [
    mockTransaccionCredito,
    mockTransaccionDebito,
    mockTransaccionPagada,
    mockTransaccionCancelada,
]

// ========== Paged Response ==========

export const mockTransaccionesPagedResponse: WalletTransaccionesPagedResponse = {
    items: mockTransacciones,
    totalCount: 4,
    page: 1,
    pageSize: 10,
    totalPages: 1,
}

export const mockTransaccionesPagedResponse_Empty: WalletTransaccionesPagedResponse = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 10,
    totalPages: 0,
}

export const mockTransaccionesPagedResponse_MultiPage: WalletTransaccionesPagedResponse = {
    items: mockTransacciones,
    totalCount: 25,
    page: 1,
    pageSize: 10,
    totalPages: 3,
}

// ========== Solicitar Cobro Response ==========

export const mockSolicitarCobroResponse: SolicitarCobroResponse = {
    transaccionId: "tx-new-001",
    importe: 50.00,
    monedaNombre: "EUR",
    estadoTransaccionNombre: "Pendiente",
    saldoRestante: 100.50,
    fechaCreacion: "2026-03-01T09:00:00Z",
}
