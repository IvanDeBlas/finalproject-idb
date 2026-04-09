// Artista domain types

export interface Artista {
  id: string
  userId: string
  nombreArtistico: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
  generoMusical?: string
  createdAt: Date
  updatedAt: Date
}

export interface CreateArtistaData {
  nombreArtistico: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
  generoMusical?: string
}

export interface UpdateArtistaData {
  nombreArtistico?: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
  generoMusical?: string
}

export interface IArtistaRepository {
  getById(id: string): Promise<Artista>
  getMyProfile(): Promise<Artista | null>
  create(data: CreateArtistaData): Promise<Artista>
  update(id: string, data: UpdateArtistaData): Promise<Artista>
}
