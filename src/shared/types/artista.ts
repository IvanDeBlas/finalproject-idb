export interface Artista {
  id: string
  userId: string
  nombreArtistico: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
  generoMusical?: string
  redesSociales?: RedesSociales
  fechaCreacion: string
  fechaActualizacion?: string
}

export interface ArtistaDto {
  id: string
  userId: string
  nombreArtistico: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
  generoMusical?: string
  redesSociales?: RedesSociales
  fechaCreacion: string
  fechaActualizacion?: string
}

export interface ArtistaListItem {
  id: string
  nombreArtistico: string
  imagenUrl?: string
  ciudad?: string
  pais?: string
}

export interface CreateArtistaDto {
  nombreArtistico: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
  generoMusical?: string
  redesSociales?: RedesSociales
}

export interface UpdateArtistaDto {
  nombreArtistico?: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
  generoMusical?: string
  redesSociales?: RedesSociales
}

export interface RedesSociales {
  instagram?: string
  twitter?: string
  youtube?: string
  spotify?: string
  website?: string
}
