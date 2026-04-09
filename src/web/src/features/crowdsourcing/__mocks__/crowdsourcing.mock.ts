import type {
    PlantillaProyectoList,
    PlantillaProyecto,
    PlantillaProyectoNecesidad,
    GenerarNecesidadesResult,
} from "../domain"

export const mockNecesidadAlta: PlantillaProyectoNecesidad = {
    id: "nec-001",
    fase: "Preproduccion",
    titulo: "Productor Musical",
    descripcion: "Desarrollo de arreglos, estructura y direccion musical",
    rolProfesional: {
        id: 1,
        nombre: "Productor Musical",
        descripcion: "Profesional que guia el proceso creativo y tecnico",
        categoriaRolId: 1,
        modalidadCobro: "Por proyecto",
    },
    precioMinOrientativo: 500,
    precioMaxOrientativo: 1500,
    moneda: 1,
    prioridad: "Alta",
    orden: 1,
}

export const mockNecesidadMedia: PlantillaProyectoNecesidad = {
    id: "nec-002",
    fase: "Grabacion",
    titulo: "Ingeniero de Grabacion",
    descripcion: "Grabacion profesional de instrumentos y voces",
    rolProfesional: {
        id: 2,
        nombre: "Ingeniero de Grabacion",
        descripcion: "Tecnico especializado en captura de audio",
        categoriaRolId: 2,
        modalidadCobro: "Por dia",
    },
    precioMinOrientativo: 800,
    precioMaxOrientativo: 2000,
    moneda: 1,
    prioridad: "Media",
    orden: 2,
}

export const mockNecesidadBaja: PlantillaProyectoNecesidad = {
    id: "nec-003",
    fase: "Mezcla y Master",
    titulo: "Ingeniero de Mezcla",
    descripcion: "Mezcla profesional del EP",
    rolProfesional: {
        id: 3,
        nombre: "Ingeniero de Mezcla",
        descripcion: "Especialista en balance y procesamiento de audio",
        categoriaRolId: 2,
        modalidadCobro: "Por cancion",
    },
    precioMinOrientativo: 600,
    precioMaxOrientativo: 1800,
    moneda: 1,
    prioridad: "Baja",
    orden: 3,
}

export const mockTemplatesList: PlantillaProyectoList[] = [
    {
        id: "tpl-001",
        nombre: "Produccion de EP",
        descripcion: "Plantilla completa para producir un EP de 4-6 canciones",
        icono: "music",
        orden: 1,
        precioMinTotal: 3000,
        precioMaxTotal: 8000,
        moneda: 1,
        cantidadNecesidades: 8,
        fases: ["Preproduccion", "Grabacion", "Mezcla y Master", "Promocion"],
    },
    {
        id: "tpl-002",
        nombre: "Produccion de Album",
        descripcion: "Plantilla para producir un album de 10-12 canciones",
        icono: "disc",
        orden: 2,
        precioMinTotal: 8000,
        precioMaxTotal: 25000,
        moneda: 1,
        cantidadNecesidades: 12,
        fases: ["Preproduccion", "Grabacion", "Mezcla y Master", "Diseno", "Promocion"],
    },
    {
        id: "tpl-003",
        nombre: "Produccion de Sencillo",
        descripcion: "Plantilla agil para producir 1-2 canciones",
        icono: "video",
        orden: 3,
        precioMinTotal: 1000,
        precioMaxTotal: 3500,
        moneda: 1,
        cantidadNecesidades: 5,
        fases: ["Grabacion", "Mezcla y Master", "Promocion"],
    },
]

export const mockTemplateDetail: PlantillaProyecto = {
    id: "tpl-001",
    nombre: "Produccion de EP",
    descripcion: "Plantilla completa para producir un EP de 4-6 canciones",
    icono: "music",
    orden: 1,
    necesidades: [mockNecesidadAlta, mockNecesidadMedia, mockNecesidadBaja],
    resumen: {
        precioMinTotal: 1900,
        precioMaxTotal: 5300,
        moneda: 1,
        cantidadNecesidadesAlta: 1,
        cantidadNecesidadesMedia: 1,
        cantidadNecesidadesBaja: 1,
    },
}

export const mockGenerarResult: GenerarNecesidadesResult = {
    necesidadesCreadas: 2,
    necesidadIds: ["gen-001", "gen-002"],
    presupuestoTotalMin: 1300,
    presupuestoTotalMax: 3500,
    moneda: 1,
}
