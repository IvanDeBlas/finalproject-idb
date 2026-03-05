import type { Reward } from "@shared/types/reward"

export const mockRewardDigital: Reward = {
    id: "123e4567-e89b-12d3-a456-426614174000",
    campaniaId: "550e8400-e29b-41d4-a716-446655440000",
    tipoRewardId: 1,
    nombre: "Descarga Digital",
    descripcion: "Acceso anticipado al album completo en formato digital FLAC + MP3",
    importeMinimo: 10,
    monedaId: 1,
    esAddOn: false,
    cantidadMaxima: undefined,
    cantidadPorBacker: 1,
    incluyeEnvioFisico: false,
    tiempoEntregaEstimado: "Inmediato tras finalizar campania",
    orden: 1,
    esActivo: true,
    fechaCreacion: "2026-02-10T10:00:00Z",
    fechaActualizacion: undefined,
}

export const mockRewardFisico: Reward = {
    id: "223e4567-e89b-12d3-a456-426614174001",
    campaniaId: "550e8400-e29b-41d4-a716-446655440000",
    tipoRewardId: 2,
    nombre: "CD Fisico Firmado",
    descripcion: "CD fisico del album con firma del artista + booklet dedicado",
    importeMinimo: 25,
    monedaId: 1,
    esAddOn: false,
    cantidadMaxima: 200,
    cantidadPorBacker: 1,
    incluyeEnvioFisico: true,
    tiempoEntregaEstimado: "Marzo 2026",
    orden: 2,
    esActivo: true,
    fechaCreacion: "2026-02-10T10:00:00Z",
    fechaActualizacion: undefined,
}

export const mockRewardLimitedStock: Reward = {
    id: "323e4567-e89b-12d3-a456-426614174002",
    campaniaId: "550e8400-e29b-41d4-a716-446655440000",
    tipoRewardId: 2,
    nombre: "Vinilo Edicion Limitada",
    descripcion: "Vinilo en color especial + poster exclusivo + todo lo anterior",
    importeMinimo: 50,
    monedaId: 1,
    esAddOn: false,
    cantidadMaxima: 100,
    cantidadPorBacker: 1,
    incluyeEnvioFisico: true,
    tiempoEntregaEstimado: "Marzo 2026",
    orden: 3,
    esActivo: true,
    fechaCreacion: "2026-02-10T10:00:00Z",
    fechaActualizacion: undefined,
}

export const mockRewardSoldOut: Reward = {
    id: "423e4567-e89b-12d3-a456-426614174003",
    campaniaId: "550e8400-e29b-41d4-a716-446655440000",
    tipoRewardId: 3,
    nombre: "Paquete VIP",
    descripcion: "Meet & greet privado + vinilo + mercancia exclusiva",
    importeMinimo: 100,
    monedaId: 1,
    esAddOn: false,
    cantidadMaxima: 0,
    cantidadPorBacker: 1,
    incluyeEnvioFisico: false,
    tiempoEntregaEstimado: "Abril 2026",
    orden: 4,
    esActivo: true,
    fechaCreacion: "2026-02-10T10:00:00Z",
    fechaActualizacion: undefined,
}

export const mockRewardsList: Reward[] = [
    mockRewardDigital,
    mockRewardFisico,
    mockRewardLimitedStock,
    mockRewardSoldOut,
]
