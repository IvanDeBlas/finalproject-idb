# Validacion QA: Gestionar Necesidades de Crowdsourcing (Admin Dashboard)

**Fecha:** 2026-02-16
**Feature:** cs-gestionar-necesidades
**Target:** src/admin
**Baseline:** docs/user-stories/cs-gestionar-necesidades/feature-spec.md

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Criterios de Aceptacion | 12 |
| Cubiertos por Contratos | 12 |
| Parcialmente Cubiertos | 0 |
| No Cubiertos | 0 |
| **Score de Cobertura (Contratos)** | **100%** |
| **Score de Cobertura (Frontend Plan)** | **0%** |
| **Score Global** | **50%** |

**Estado:** REQUIERE IMPLEMENTACION

**Veredicto:** Los contratos (shared, backend, UI/UX) cubren todos los criterios de aceptacion al 100%, pero NO existe plan de implementacion frontend. Se requiere generar:
- Frontend plan (arquitectura, componentes, hooks)
- UI design plan (componentes concretos)
- Test strategy plan (tests especificos)

---

## 2. Criterios de Aceptacion (feature-spec.md)

### Fuente: docs/user-stories/cs-gestionar-necesidades/feature-spec.md

| ID | Criterio | Tipo | Proyecto |
|----|----------|------|----------|
| AC-CS02-1 | Crear necesidad con campos obligatorios validados (Titulo 5-200, TipoNecesidad, Modalidad, Proyecto). Ubicacion requerida si modalidad != Remoto. PresupuestoMax >= PresupuestoMin. FechaLimite > hoy+1. | Funcional | Backend + Admin |
| AC-CS02-2 | Necesidad creada con estado Abierta, aparece en listado del artista y publico, toast success. | Funcional | Backend + Admin |
| AC-CS02-3 | ArtistaId auto-asignado desde JWT. ProyectoArtisticoId validado (403 si no pertenece). | Seguridad | Backend |
| AC-CS02-4 | Necesidad desde template pre-rellena campos pero son editables (query param ?fromTemplate={id}). | Funcional | Admin |
| AC-CS02-5 | Listado paginado con badges de color por estado, contador propuestas, filtros (estado + search). | UI/UX | Admin |
| AC-CS02-6 | Contador de propuestas (COUNT de PropuestaCrowdsourcing) visible en cada card. | Funcional | Backend + Admin |
| AC-CS02-7 | Filtros server-side por estado (multi-select) y texto (busca en titulo + descripcion). | Funcional | Backend + Admin |
| AC-CS02-8 | Empty state con ilustracion y CTAs si no hay necesidades. | UI/UX | Admin |
| AC-CS02-9 | Solo editar necesidad Abierta. Banner warning si tiene propuestas. Actualiza FechaActualizacion. Boton editar oculto si estado != Abierta. | Funcional + UI/UX | Backend + Admin |
| AC-CS02-10 | Cerrar necesidad auto-rechaza propuestas Pendiente. Dialogo confirmacion previo. | Funcional + UI/UX | Backend + Admin |
| AC-CS02-11 | Click en card navega a detalle con propuestas. GET /necesidades/{id} retorna necesidad + propuestas. | Funcional | Backend + Admin |
| AC-CS02-12 | Necesidad cerrada no aparece en listado publico pero si en historial artista. | Funcional | Backend |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio | contracts-plan | ui-ux | frontend-plan | ui-design | test-strategy | Estado |
|----|----------|----------------|-------|---------------|-----------|---------------|--------|
| AC-CS02-1 | Validacion campos crear necesidad | CUBIERTO (schemas Zod + backend validators) | CUBIERTO (formulario) | GAP | GAP | GAP | PARCIAL |
| AC-CS02-2 | Estado Abierta + listado + toast | CUBIERTO (API POST, GET) | CUBIERTO (listado + toast) | GAP | GAP | GAP | PARCIAL |
| AC-CS02-3 | ArtistaId JWT + validar proyecto | CUBIERTO (Auth + 403) | N/A | GAP | N/A | GAP | PARCIAL |
| AC-CS02-4 | Pre-rellenar desde template | CUBIERTO (query param) | CUBIERTO (form con datos) | GAP | GAP | GAP | PARCIAL |
| AC-CS02-5 | Listado paginado + badges + filtros | CUBIERTO (API + tipos) | CUBIERTO (cards + filtros UI) | GAP | GAP | GAP | PARCIAL |
| AC-CS02-6 | Contador propuestas en card | CUBIERTO (numeroPropuestas en DTO) | CUBIERTO (badge contador) | GAP | GAP | GAP | PARCIAL |
| AC-CS02-7 | Filtros server-side (estado + search) | CUBIERTO (query params) | CUBIERTO (Select + Input) | GAP | GAP | GAP | PARCIAL |
| AC-CS02-8 | Empty state necesidades | N/A | CUBIERTO (ilustracion + CTAs) | GAP | GAP | GAP | PARCIAL |
| AC-CS02-9 | Editar solo Abierta + banner warning | CUBIERTO (API 400 + NumeroPropuestas) | CUBIERTO (banner + boton oculto) | GAP | GAP | GAP | PARCIAL |
| AC-CS02-10 | Cerrar necesidad + rechazar propuestas | CUBIERTO (PATCH /cerrar) | CUBIERTO (dialogo confirmacion) | GAP | GAP | GAP | PARCIAL |
| AC-CS02-11 | Click card → detalle con propuestas | CUBIERTO (GET /necesidades/{id}) | CUBIERTO (detalle + propuestas) | GAP | GAP | GAP | PARCIAL |
| AC-CS02-12 | Cerrada no en publico, si en historial | CUBIERTO (filtro backend) | N/A | GAP | N/A | GAP | PARCIAL |

**Leyenda:**
- **CUBIERTO:** Requisito completamente especificado en contratos/UI
- **PARCIAL:** Especificado en contratos pero sin plan de implementacion frontend
- **GAP:** No especificado en ningun plan
- **N/A:** No aplica a este plan (ej: backend-only)

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura | Estado |
|----|----------|-----------|--------|
| NFR-01 | Responsive mobile (1/2/3 cols segun breakpoint) | ui-ux: Especificado (< 768px, 768-1024px, > 1024px) | CUBIERTO |
| NFR-02 | Accesibilidad WCAG AA (ARIA labels, focus states) | ui-ux: Especificado (contraste 4.5:1, ARIA, keyboard nav) | CUBIERTO |
| NFR-03 | Validacion en tiempo real (Zod + react-hook-form) | contracts: Schemas Zod con refines, ui-ux: validacion en blur | CUBIERTO |
| NFR-04 | Debounce 300ms en search | ui-ux: Especificado | CUBIERTO |
| NFR-05 | Loading skeletons (3-4 cards) | ui-ux: Especificado (shimmer effect) | CUBIERTO |
| NFR-06 | Toast notifications (success/error) | ui-ux: Especificado (12 toast messages) | CUBIERTO |
| NFR-07 | Dark theme consistente | ui-ux: Design tokens definidos | CUBIERTO |
| NFR-08 | Animaciones smooth (150-400ms) | ui-ux: Especificado (hover, focus, shake, etc) | CUBIERTO |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos (Bloquean implementacion)

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-001 | Todos (AC-CS02-1 a AC-CS02-12) | No existe frontend-plan.md con arquitectura de componentes, hooks, services | ALTO - Bloquea desarrollo | **ACCION REQUERIDA:** Generar frontend-plan.md con: (1) Arquitectura de features, (2) Componentes (NecesidadesListPage, NuevaNecesidadPage, NecesidadDetailPage, etc), (3) Hooks (useMisNecesidades, useCreateNecesidad, etc), (4) Services (necesidad.service.ts), (5) Routing |
| GAP-002 | AC-CS02-5, AC-CS02-6, AC-CS02-8 | No existe ui-design.md con componentes concretos de shadcn/ui | ALTO - Sin especificacion de componentes | **ACCION REQUERIDA:** Generar ui-design.md con: (1) NecesidadCard component spec, (2) EstadoBadge component, (3) NecesidadFilters component, (4) EmptyStateNecesidades component, (5) PropuestaCard component, (6) CerrarNecesidadDialog component |
| GAP-003 | Todos | No existe test-strategy.md con tests especificos | ALTO - Sin estrategia de testing | **ACCION REQUERIDA:** Generar test-strategy.md con: (1) Unit tests de hooks (useMisNecesidades, useCreateNecesidad), (2) Unit tests de schemas Zod (validacion refines), (3) Integration tests de forms (crear, editar), (4) E2E tests de flujos completos |

### 4.2 Gaps Mayores (Impactan calidad)

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-004 | AC-CS02-4 | Integracion con wizard de templates (US-CS-01) no documentada en frontend-plan | MEDIO - Dependencia entre features | Documentar en frontend-plan: (1) Como detectar query param ?fromTemplate={id}, (2) Como pre-rellenar form desde datos template, (3) Navegacion desde wizard templates |
| GAP-005 | AC-CS02-9 | Logica condicional de boton Editar no especificada en ui-design | MEDIO - Comportamiento UI ambiguo | Especificar en ui-design: (1) Condicion para mostrar/ocultar boton Editar, (2) Tooltip explicativo si disabled, (3) Manejo de error 400 si intenta editar via API directamente |
| GAP-006 | AC-CS02-10 | Flujo de rechazo automatico de propuestas no documentado en frontend-plan | MEDIO - Backend-heavy pero impacta UX | Documentar en frontend-plan: (1) Mensaje toast con contador de propuestas rechazadas, (2) Actualizacion de listado tras cerrar, (3) Manejo de error si no se pueden rechazar propuestas |

### 4.3 Gaps Menores (Nice to have)

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-007 | AC-CS02-5 | Fecha limite proxima (< 7 dias) no tiene badge visual especificado en ui-design | BAJO - UX mejorada | Agregar en ui-design: Badge "Cierra pronto" amarillo si < 7 dias, "Urgente" rojo si < 3 dias |
| GAP-008 | AC-CS02-11 | Renderizado de propuestas en detalle no especifica ordenamiento | BAJO - Orden visual | Especificar en ui-design: Ordenar propuestas por fechaCreacion DESC (mas recientes primero) |

---

## 5. Cobertura de Contratos vs Implementacion

### 5.1 Contratos Shared (contracts-plan.md)

**Estado:** COMPLETO

| Elemento | Especificado | Notas |
|----------|--------------|-------|
| Types TypeScript | SI (9 interfaces + 2 unions) | NecesidadCrowdsourcing, NecesidadCrowdsourcingList, PropuestaCrowdsourcing, CreateNecesidadRequest, UpdateNecesidadRequest, CerrarNecesidadRequest, *Result types |
| Schemas Zod | SI (3 schemas con 5 refines) | createNecesidadSchema, updateNecesidadSchema, cerrarNecesidadSchema |
| Constants | SI (15 grupos) | QUERY_KEYS, API_ROUTES, APP_ROUTES, ESTADO_NECESIDAD, MODALIDAD_TRABAJO, MONEDA, badges, labels, icons |
| Utilidades | SI (1 formatter + 12 errors) | formatPresupuesto(), NECESIDAD_ERROR_MESSAGES |
| Validaciones compartidas | SI (tabla completa) | Mapeo backend FluentValidation ↔ frontend Zod con mensajes identicos |

**Fortalezas:**
- Schemas Zod con refines complejos cubren todas las reglas de negocio (presupuesto, ubicacion condicional, fechas)
- Constants exhaustivos para estados, modalidades, monedas
- Error messages alineados con backend (codigos 1011, 1012, 2009, 4001, 4002)
- Formatter de presupuesto con manejo de edge cases

**Debilidades:**
- Sin plan de implementacion frontend que USE estos contratos
- Sin especificacion de como aplicar schemas Zod en formularios (react-hook-form resolver)

### 5.2 UI/UX (ui-ux.md)

**Estado:** COMPLETO

| Pantalla | Especificada | Componentes | Interacciones | Responsive | Accesibilidad |
|----------|--------------|-------------|---------------|------------|---------------|
| 1. Mis Necesidades (Listado) | SI | NecesidadCard, EstadoBadge, Filters, Empty State | Click card → detalle, filtros, paginacion | 1/2/3 cols | ARIA labels, focus |
| 2. Crear Necesidad (Form) | SI | Form sections, inputs, selects, datepickers | Validacion real-time, conditional fields | 1 col mobile, 2 cols desktop | Labels, required, errors |
| 3. Editar Necesidad (Form) | SI | Same as crear + banner warning | Pre-rellenado, tipo disabled | Same as crear | Same as crear |
| 4. Detalle Necesidad | SI | Header, Details, PropuestaCard | Ver propuestas, aceptar/rechazar | Info grid responsive | ARIA, keyboard nav |
| 5. Dialogo Cierre | SI | Dialog, warning alert, textarea | Confirmacion, motivo opcional | Modal responsive | Focus trap, escape |

**Fortalezas:**
- Mockups de layout ASCII detallados
- Design tokens definidos (colores, tipografia, espaciado)
- Responsive breakpoints especificados (< 768px, 768-1024px, > 1024px)
- Accesibilidad WCAG AA (contraste, ARIA, keyboard)
- Animaciones y transiciones documentadas
- Loading skeletons especificados

**Debilidades:**
- Sin componentes React concretos (solo descripciones visuales)
- Sin props de componentes especificados
- Sin estado local de componentes documentado

---

## 6. Validacion de Tests (test-strategy.md)

**Estado:** NO EXISTE - GAP CRITICO

### Tests Requeridos por AC

| Criterio | Test Requerido | Tipo | Razon | Estado |
|----------|----------------|------|-------|--------|
| AC-CS02-1 | Validacion campos obligatorios (titulo, tipo, modalidad, proyecto) | Unit | Verificar schema Zod rechaza campos vacios | GAP |
| AC-CS02-1 | Validacion titulo min 5 chars | Unit | Verificar schema Zod valida min length | GAP |
| AC-CS02-1 | Validacion presupuesto max >= min | Unit | Verificar refine de Zod funciona | GAP |
| AC-CS02-1 | Validacion ubicacion requerida si Presencial/Hibrido | Unit | Verificar refine condicional de Zod | GAP |
| AC-CS02-1 | Validacion fecha limite > hoy | Unit | Verificar refine de fecha en Zod | GAP |
| AC-CS02-2 | Crear necesidad muestra toast success | Integration | Verificar useMutation actualiza cache y muestra toast | GAP |
| AC-CS02-2 | Necesidad aparece en listado tras crear | Integration | Verificar invalidacion de query cache | GAP |
| AC-CS02-3 | Error 403 si proyecto no pertenece al artista | Integration | Verificar manejo de error en API call | GAP |
| AC-CS02-4 | Pre-rellenar form desde template | Integration | Verificar deteccion de query param y fetch de template | GAP |
| AC-CS02-5 | Badges de estado con colores correctos | Unit | Verificar EstadoBadge renderiza color segun ESTADO_NECESIDAD_BADGES | GAP |
| AC-CS02-5 | Filtro de estado actualiza listado | Integration | Verificar cambio de filtro invalida query y refetch | GAP |
| AC-CS02-6 | Contador de propuestas visible | Unit | Verificar NecesidadCard renderiza numeroPropuestas | GAP |
| AC-CS02-7 | Search con debounce 300ms | Unit | Verificar useDebounce hook funciona correctamente | GAP |
| AC-CS02-7 | Filtros aplican query params en API call | Integration | Verificar useMisNecesidades construye URL con filtros | GAP |
| AC-CS02-8 | Empty state visible si no hay necesidades | Unit | Verificar componente renderiza si items.length === 0 | GAP |
| AC-CS02-9 | Boton Editar oculto si estado != Abierta | Unit | Verificar NecesidadCard oculta boton si estadoNecesidadId !== 1 | GAP |
| AC-CS02-9 | Banner warning si necesidad tiene propuestas | Unit | Verificar EditarNecesidadPage muestra banner si numeroPropuestas > 0 | GAP |
| AC-CS02-9 | Error 400 al editar necesidad cerrada | Integration | Verificar manejo de error y toast | GAP |
| AC-CS02-10 | Dialogo confirmacion antes de cerrar | Unit | Verificar CerrarNecesidadDialog renderiza warning | GAP |
| AC-CS02-10 | Toast muestra propuestas rechazadas | Integration | Verificar useCerrarNecesidad muestra toast con propuestasRechazadas | GAP |
| AC-CS02-11 | Click en card navega a detalle | Unit | Verificar onClick de NecesidadCard llama navigate() | GAP |
| AC-CS02-11 | Detalle carga propuestas | Integration | Verificar useNecesidadById fetches propuestas anidadas | GAP |
| AC-CS02-12 | Necesidad cerrada en historial pero no en publico | E2E | Verificar flujo completo (crear, cerrar, verificar listado) | GAP |

**Total tests requeridos:** 23 tests minimos

**Tests faltantes criticos:**
- Validacion de schemas Zod (5 tests de refines)
- Mutations con invalidacion de cache (3 tests)
- Manejo de errores API (3 tests)
- Renderizado condicional de UI (5 tests)

---

## 7. Requisitos de Testing por AC

### AC-CS02-1: Validacion campos crear necesidad

**Tests especificos:**

1. **Unit: Schema Zod - Campos requeridos**
   ```typescript
   it('rechaza titulo vacio', () => {
     const result = createNecesidadSchema.safeParse({ titulo: '' });
     expect(result.success).toBe(false);
     expect(result.error.errors[0].message).toBe('El título es obligatorio');
   });
   ```

2. **Unit: Schema Zod - Titulo min 5 chars**
   ```typescript
   it('rechaza titulo con menos de 5 caracteres', () => {
     const result = createNecesidadSchema.safeParse({ titulo: 'Hola' });
     expect(result.success).toBe(false);
     expect(result.error.errors[0].message).toBe('El título debe tener al menos 5 caracteres');
   });
   ```

3. **Unit: Schema Zod - Presupuesto max >= min**
   ```typescript
   it('rechaza presupuesto max menor que min', () => {
     const result = createNecesidadSchema.safeParse({
       presupuestoMin: 500,
       presupuestoMax: 200,
     });
     expect(result.success).toBe(false);
     expect(result.error.errors[0].path).toEqual(['presupuestoMax']);
   });
   ```

4. **Unit: Schema Zod - Ubicacion requerida si Presencial**
   ```typescript
   it('rechaza modalidad Presencial sin ubicacion', () => {
     const result = createNecesidadSchema.safeParse({
       modalidadTrabajoId: 1, // Presencial
       ubicacionCiudad: undefined,
     });
     expect(result.success).toBe(false);
     expect(result.error.errors[0].message).toBe('La ubicación es obligatoria para modalidad Presencial o Híbrida');
   });
   ```

5. **Integration: Form submit con datos validos**
   ```typescript
   it('crea necesidad con datos validos', async () => {
     const { user } = render(<NuevaNecesidadPage />);

     await user.type(screen.getByLabelText('Título *'), 'Mezcla de pistas para EP');
     await user.selectOptions(screen.getByLabelText('Tipo necesidad*'), '3'); // Produccion
     await user.selectOptions(screen.getByLabelText('Modalidad*'), '2'); // Remoto
     await user.selectOptions(screen.getByLabelText('Proyecto Artístico *'), 'proyecto-1');

     await user.click(screen.getByRole('button', { name: 'Publicar Necesidad' }));

     await waitFor(() => {
       expect(screen.getByText('Necesidad publicada correctamente')).toBeInTheDocument();
     });
   });
   ```

### AC-CS02-5: Listado paginado con badges

**Tests especificos:**

1. **Unit: EstadoBadge - Color segun estado**
   ```typescript
   it('renderiza badge verde para estado Abierta', () => {
     render(<EstadoBadge estado="Abierta" />);
     const badge = screen.getByText('Abierta');
     expect(badge).toHaveClass('bg-green-900/20', 'text-green-400');
   });
   ```

2. **Integration: Listado carga y renderiza cards**
   ```typescript
   it('carga y renderiza necesidades paginadas', async () => {
     const { result } = renderHook(() => useMisNecesidades({ page: 1 }));

     await waitFor(() => expect(result.current.isSuccess).toBe(true));

     expect(result.current.data.items).toHaveLength(10);
     expect(result.current.data.totalCount).toBe(25);
     expect(result.current.data.totalPages).toBe(3);
   });
   ```

3. **Integration: Filtro de estado actualiza listado**
   ```typescript
   it('filtra por estado Abierta', async () => {
     const { rerender } = render(<MisNecesidadesPage />);

     const filtroEstado = screen.getByLabelText('Estado');
     await userEvent.selectOptions(filtroEstado, '1'); // Abierta

     await waitFor(() => {
       const cards = screen.getAllByRole('article');
       cards.forEach(card => {
         expect(within(card).getByText('Abierta')).toBeInTheDocument();
       });
     });
   });
   ```

### AC-CS02-9: Editar solo Abierta

**Tests especificos:**

1. **Unit: Boton Editar oculto si estado != Abierta**
   ```typescript
   it('oculta boton Editar si necesidad cerrada', () => {
     const necesidadCerrada = { ...mockNecesidad, estadoNecesidadId: 3 }; // Cerrada
     render(<NecesidadCard necesidad={necesidadCerrada} />);

     expect(screen.queryByRole('button', { name: 'Editar' })).not.toBeInTheDocument();
   });
   ```

2. **Unit: Banner warning si tiene propuestas**
   ```typescript
   it('muestra banner warning si necesidad tiene propuestas', () => {
     const necesidadConPropuestas = { ...mockNecesidad, numeroPropuestas: 5 };
     render(<EditarNecesidadPage necesidadId={necesidadConPropuestas.id} />);

     expect(screen.getByText(/Esta necesidad ya tiene 5 propuestas/i)).toBeInTheDocument();
   });
   ```

3. **Integration: Error 400 al editar necesidad cerrada**
   ```typescript
   it('muestra error al intentar editar necesidad cerrada', async () => {
     server.use(
       http.put('/api/crowdsourcing/necesidades/:id', () => {
         return HttpResponse.json(
           { messages: [{ errorCode: '4001', message: 'Solo se pueden editar necesidades en estado Abierta' }] },
           { status: 400 }
         );
       })
     );

     render(<EditarNecesidadPage necesidadId="id-cerrada" />);
     await user.click(screen.getByRole('button', { name: 'Guardar Cambios' }));

     await waitFor(() => {
       expect(screen.getByText('Solo se pueden editar necesidades en estado Abierta')).toBeInTheDocument();
     });
   });
   ```

### AC-CS02-10: Cerrar necesidad

**Tests especificos:**

1. **Unit: Dialogo confirmacion muestra warning**
   ```typescript
   it('muestra dialogo con advertencia de propuestas rechazadas', () => {
     render(<CerrarNecesidadDialog open={true} necesidadId="id" />);

     expect(screen.getByText(/Al cerrar esta necesidad, las propuestas pendientes serán rechazadas/i)).toBeInTheDocument();
   });
   ```

2. **Integration: Toast muestra contador propuestas rechazadas**
   ```typescript
   it('muestra toast con propuestas rechazadas al cerrar', async () => {
     const { result } = renderHook(() => useCerrarNecesidad('id'));

     act(() => {
       result.current.mutate({ motivo: 'Ya no necesito' });
     });

     await waitFor(() => expect(result.current.isSuccess).toBe(true));

     expect(screen.getByText(/3 propuestas rechazadas/i)).toBeInTheDocument();
   });
   ```

---

## 8. Riesgos de Calidad

### 8.1 Riesgos Criticos (Bloquean MVP)

| Riesgo | Probabilidad | Impacto | Mitigacion |
|--------|--------------|---------|------------|
| **Sin plan frontend = sin implementacion coherente** | ALTA (100%) | CRITICO | Generar frontend-plan.md ANTES de implementar |
| **Validaciones Zod no probadas = bugs en produccion** | ALTA | CRITICO | Crear test suite de schemas Zod con 100% coverage de refines |
| **Integracion con templates (AC-CS02-4) mal implementada** | MEDIA | ALTO | Documentar flujo completo desde wizard templates hasta form pre-rellenado |
| **Error 400/403 no manejados = UX rota** | MEDIA | ALTO | Documentar manejo de errores en frontend-plan + tests de integracion |

### 8.2 Riesgos Mayores (Impactan UX)

| Riesgo | Probabilidad | Impacto | Mitigacion |
|--------|--------------|---------|------------|
| **Botones de accion (Editar/Cerrar) visibles cuando no deberian** | MEDIA | MEDIO | Especificar logica condicional exacta en ui-design + unit tests |
| **Listado no actualiza tras crear/editar/cerrar** | MEDIA | MEDIO | Documentar invalidacion de cache en frontend-plan + integration tests |
| **Search con debounce no funciona correctamente** | BAJA | MEDIO | Implementar custom hook useDebounce + unit test |
| **Fecha limite proxima no se destaca visualmente** | BAJA | BAJO | Agregar badge warning a ui-design + unit test |

### 8.3 Riesgos de Performance

| Riesgo | Probabilidad | Impacto | Mitigacion |
|--------|--------------|---------|------------|
| **Listado con 100+ necesidades carga lento** | MEDIA | MEDIO | Confirmar paginacion server-side (pageSize max 50) |
| **Queries duplicados de maestras (TipoNecesidad, Modalidad, Moneda)** | ALTA | BAJO | Implementar cache persistente de maestras (staleTime: Infinity) |
| **Re-renders innecesarios de NecesidadCard** | BAJA | BAJO | Usar React.memo en NecesidadCard + useMemo para computed values |

---

## 9. Recomendaciones

### 9.1 Acciones Requeridas (Critico - Bloquean implementacion)

#### 1. GENERAR frontend-plan.md

**Archivo:** `plans/cs-gestionar-necesidades/frontend-admin/frontend-plan.md`

**Contenido minimo:**

```markdown
# Frontend Plan: Gestionar Necesidades (Admin Dashboard)

## 1. Arquitectura de Features

src/admin/src/features/crowdsourcing/necesidades/
├── components/
│   ├── NecesidadCard.tsx
│   ├── EstadoBadge.tsx
│   ├── NecesidadFilters.tsx
│   ├── EmptyStateNecesidades.tsx
│   ├── PropuestaCard.tsx
│   └── CerrarNecesidadDialog.tsx
├── hooks/
│   ├── useMisNecesidades.ts         # useQuery con paginacion + filtros
│   ├── useNecesidadById.ts          # useQuery detalle + propuestas
│   ├── useCreateNecesidad.ts        # useMutation + invalidation
│   ├── useUpdateNecesidad.ts        # useMutation + invalidation
│   ├── useCerrarNecesidad.ts        # useMutation + invalidation
│   ├── useTiposNecesidad.ts         # useQuery maestras (cache persistente)
│   ├── useModalidadesTrabajo.ts     # useQuery maestras
│   ├── useMonedas.ts                # useQuery maestras
│   └── useDebounce.ts               # Custom hook para search
├── pages/
│   ├── MisNecesidadesPage.tsx       # /crowdsourcing/necesidades
│   ├── NuevaNecesidadPage.tsx       # /crowdsourcing/necesidades/nueva
│   ├── EditarNecesidadPage.tsx      # /crowdsourcing/necesidades/:id/editar
│   └── NecesidadDetailPage.tsx      # /crowdsourcing/necesidades/:id
├── services/
│   └── necesidad.service.ts         # API calls
└── types/
    └── index.ts                     # Re-export de shared types

## 2. Componentes Principales

### NecesidadesListPage (MisNecesidadesPage.tsx)

**Props:** Ninguna (usa useSearchParams para filtros)

**Estado local:**
- `page` (number) - Pagina actual
- `estadoFilter` (number[] | undefined) - Filtro de estado multi-select
- `searchQuery` (string) - Busqueda de texto (debounced)

**Hooks:**
- `useMisNecesidades({ page, estado: estadoFilter, search: debouncedSearch })`
- `useDebounce(searchQuery, 300)`

**Renderizado:**
- Header con titulo + botones [+ Nueva Necesidad] [Usar Plantilla]
- Card de filtros (Select estado + Input search)
- Loading: Skeleton de 3-4 cards
- Empty: EmptyStateNecesidades si items.length === 0
- Grid: Map de NecesidadCard
- Pagination

### NuevaNecesidadPage / EditarNecesidadPage

**Props:** `necesidadId?: string` (solo en Editar)

**Estado local:**
- `form` (react-hook-form con Zod resolver)

**Hooks:**
- `useTiposNecesidad()`
- `useModalidadesTrabajo()`
- `useMonedas()`
- `useProyectosArtista()` (del modulo UserAccess)
- `useCreateNecesidad()` o `useUpdateNecesidad(id)`
- `useSearchParams()` para detectar ?fromTemplate={id} (solo Crear)
- `useNecesidadById(id)` para pre-rellenar (solo Editar)

**Validacion:**
- Usar `createNecesidadSchema` o `updateNecesidadSchema`
- Validacion en blur + submit
- Mostrar errores debajo de cada input

**Campos condicionales:**
- Ubicacion (ciudad/pais) visible solo si modalidadTrabajoId === 1 o 3
- Moneda requerida si presupuestoMin o presupuestoMax presentes

**Logica especial (Editar):**
- TipoNecesidadId disabled (no editable)
- Banner warning si numeroPropuestas > 0
- Error 400 si estado != Abierta → toast error + redirect

### NecesidadDetailPage

**Props:** `necesidadId: string` (desde useParams)

**Hooks:**
- `useNecesidadById(necesidadId)` (incluye propuestas anidadas)

**Renderizado:**
- Header: titulo + estado badge + actions (Editar si Abierta, Cerrar si Abierta/En Progreso)
- Card Detalles: descripcion + info grid
- Card Propuestas: lista de PropuestaCard con acciones

## 3. Hooks de Data Fetching

### useMisNecesidades

```typescript
export function useMisNecesidades(filters?: {
  page?: number;
  pageSize?: number;
  estado?: number[];
  search?: string;
}) {
  return useQuery({
    queryKey: QUERY_KEYS.crowdsourcing.necesidades.mis.concat([filters]),
    queryFn: () => necesidadService.getMisNecesidades(filters),
    staleTime: 1000 * 60 * 5, // 5 minutos
  });
}
```

### useCreateNecesidad

```typescript
export function useCreateNecesidad() {
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const { toast } = useToast();

  return useMutation({
    mutationFn: (data: CreateNecesidadRequest) => necesidadService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.necesidades.mis });
      toast({ title: 'Necesidad publicada correctamente', variant: 'success' });
      navigate(APP_ROUTES.dashboard.crowdsourcing.necesidades);
    },
    onError: (error) => {
      const errorCode = error.response?.data?.messages?.[0]?.errorCode;
      const message = getNecesidadErrorMessage(errorCode) || 'Error al publicar la necesidad';
      toast({ title: message, variant: 'destructive' });
    },
  });
}
```

## 4. Routing

**Agregar a `src/admin/src/app/routes.tsx`:**

```typescript
{
  path: 'crowdsourcing/necesidades',
  element: <MisNecesidadesPage />,
},
{
  path: 'crowdsourcing/necesidades/nueva',
  element: <NuevaNecesidadPage />,
},
{
  path: 'crowdsourcing/necesidades/:id',
  element: <NecesidadDetailPage />,
},
{
  path: 'crowdsourcing/necesidades/:id/editar',
  element: <EditarNecesidadPage />,
},
```

## 5. Integracion con Templates (US-CS-01)

**Flujo:**
1. Artista completa wizard de templates y selecciona necesidades
2. Wizard redirige a `/crowdsourcing/necesidades/nueva?fromTemplate={templateId}`
3. NuevaNecesidadPage detecta query param
4. Fetch template data (useTemplateById)
5. Pre-rellena form con datos de template (titulo, descripcion, tipo, modalidad, presupuesto)
6. Artista puede editar campos antes de submit
7. Submit a POST /api/crowdsourcing/necesidades (mismo endpoint que creacion manual)

**Codigo:**

```typescript
// NuevaNecesidadPage.tsx
const [searchParams] = useSearchParams();
const fromTemplateId = searchParams.get('fromTemplate');
const { data: template } = useTemplateById(fromTemplateId, { enabled: !!fromTemplateId });

useEffect(() => {
  if (template) {
    form.reset({
      titulo: template.titulo,
      descripcion: template.descripcion,
      tipoNecesidadId: template.tipoNecesidadId,
      modalidadTrabajoId: template.modalidadTrabajoId,
      presupuestoMin: template.presupuestoMin,
      presupuestoMax: template.presupuestoMax,
      monedaId: template.monedaId,
      // proyectoArtisticoId lo selecciona el usuario manualmente
    });
  }
}, [template]);
```

## 6. Manejo de Errores

**Errores API:**
- 400: Validacion fallida → mostrar mensajes de error especificos en campos
- 403: Sin permiso → toast error + redirect a listado
- 404: No encontrado → toast error + redirect a listado
- 500: Error inesperado → toast generico

**Mapeo de errorCodes:**
- 1011 → "El título debe tener al menos 5 caracteres"
- 1012 → "La fecha no es válida"
- 2009 → "La necesidad no fue encontrada"
- 4001 → "Solo se pueden editar necesidades en estado Abierta"
- 4002 → "Solo se pueden cerrar necesidades en estado Abierta o En Progreso"

## 7. Performance

**Optimizaciones:**
- React.memo en NecesidadCard (evita re-renders innecesarios)
- useMemo para computed values (formatPresupuesto, fechas relativas)
- Cache persistente de maestras (staleTime: Infinity)
- Debounce de 300ms en search
- Paginacion server-side (max 50 items por pagina)
```

**Cambio:** `frontend-plan.md` es el documento clave faltante

---

#### 2. GENERAR ui-design.md

**Archivo:** `plans/cs-gestionar-necesidades/frontend-admin/ui-design.md`

**Contenido minimo:**

```markdown
# UI Design: Gestionar Necesidades (Admin Dashboard)

## Componentes Reutilizables

### NecesidadCard

**Props:**
```typescript
interface NecesidadCardProps {
  necesidad: NecesidadCrowdsourcingList;
  onClick?: () => void;
}
```

**Renderizado:**
```tsx
<Card className="p-6 hover:bg-[#1e2a42] hover:border-primary transition-all cursor-pointer">
  <div className="flex items-start justify-between mb-4">
    <EstadoBadge estado={necesidad.estadoNecesidadNombre} />
  </div>
  <h3 className="text-xl font-semibold text-white mb-3">{necesidad.titulo}</h3>
  <div className="flex items-center gap-3 text-sm text-[#94a3b8] mb-2">
    <div className="flex items-center gap-1">
      <Music className="w-4 h-4" />
      <span>{necesidad.tipoNecesidadNombre}</span>
    </div>
    <Badge variant="secondary" size="sm">
      {necesidad.modalidadTrabajoNombre}
    </Badge>
  </div>
  <div className="text-base font-medium text-white mb-3">
    {formatPresupuesto(necesidad.presupuestoMin, necesidad.presupuestoMax, necesidad.monedaId)}
  </div>
  <div className="flex items-center gap-4 text-sm text-[#94a3b8] mb-4">
    <div className="flex items-center gap-1">
      <Users className="w-4 h-4" />
      <span>{necesidad.numeroPropuestas} propuestas</span>
    </div>
    <div className="flex items-center gap-1">
      <Calendar className="w-4 h-4" />
      <span>Hace {formatDistanceToNow(new Date(necesidad.fechaCreacion))}</span>
    </div>
  </div>
  {necesidad.fechaLimitePropuestas && (
    <div className={cn(
      "text-sm mb-4",
      isBefore(new Date(necesidad.fechaLimitePropuestas), addDays(new Date(), 3)) ? "text-red-400" :
      isBefore(new Date(necesidad.fechaLimitePropuestas), addDays(new Date(), 7)) ? "text-amber-400" :
      "text-[#64748b]"
    )}>
      Límite: {format(new Date(necesidad.fechaLimitePropuestas), 'dd MMM yyyy')}
    </div>
  )}
  <div className="flex gap-2 pt-3 border-t border-[#334155]">
    <Button variant="ghost" size="sm" className="text-primary" onClick={onClick}>
      Ver Detalle
    </Button>
    {necesidad.estadoNecesidadId === ESTADO_NECESIDAD.ABIERTA && (
      <Button variant="ghost" size="sm" onClick={handleEdit}>
        Editar
      </Button>
    )}
    {(necesidad.estadoNecesidadId === ESTADO_NECESIDAD.ABIERTA ||
      necesidad.estadoNecesidadId === ESTADO_NECESIDAD.EN_PROGRESO) && (
      <Button variant="ghost" size="sm" className="text-red-400" onClick={handleCerrar}>
        Cerrar
      </Button>
    )}
  </div>
</Card>
```

### EstadoBadge

**Props:**
```typescript
interface EstadoBadgeProps {
  estado: EstadoNecesidad;
}
```

**Renderizado:**
```tsx
const colorClasses = {
  Abierta: 'bg-green-900/20 text-green-400 border-green-700',
  'En Progreso': 'bg-blue-900/20 text-blue-400 border-blue-700',
  Cerrada: 'bg-gray-900/20 text-gray-400 border-gray-700',
  Cancelada: 'bg-red-900/20 text-red-400 border-red-700',
};

<Badge variant="outline" className={colorClasses[estado]} aria-label={`Estado: ${estado}`}>
  {estado}
</Badge>
```

### CerrarNecesidadDialog

**Props:**
```typescript
interface CerrarNecesidadDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  necesidadId: string;
  onSuccess?: () => void;
}
```

**Renderizado:**
```tsx
<Dialog open={open} onOpenChange={onOpenChange}>
  <DialogContent className="max-w-md bg-[#0f1729] border-[#334155]">
    <DialogHeader>
      <DialogTitle>Cerrar Necesidad</DialogTitle>
    </DialogHeader>
    <Alert className="bg-amber-900/20 border-amber-700 mb-4">
      <AlertCircle className="w-5 h-5 text-amber-500" />
      <AlertDescription className="text-sm text-amber-300">
        Al cerrar esta necesidad, las propuestas pendientes serán rechazadas automáticamente.
      </AlertDescription>
    </Alert>
    <form onSubmit={handleSubmit}>
      <Label className="text-sm font-medium text-[#cbd5e1] mb-2 block">
        Motivo del cierre (opcional)
      </Label>
      <Textarea
        className="bg-[#1a1a2e] border-[#334155] text-white min-h-[100px]"
        placeholder="Ej: Ya no necesito este servicio"
        maxLength={500}
        {...form.register('motivo')}
      />
      <span className="text-xs text-[#64748b] mt-1">
        {form.watch('motivo')?.length || 0} / 500 caracteres
      </span>
      <DialogFooter className="flex gap-3 justify-end mt-6">
        <Button variant="outline" onClick={() => onOpenChange(false)}>
          Cancelar
        </Button>
        <Button
          type="submit"
          variant="destructive"
          disabled={isPending}
        >
          {isPending && <Loader2 className="animate-spin mr-2" />}
          Cerrar Necesidad
        </Button>
      </DialogFooter>
    </form>
  </DialogContent>
</Dialog>
```

(... y asi para EmptyStateNecesidades, NecesidadFilters, PropuestaCard ...)
```

**Cambio:** Especificar props y renderizado exacto de cada componente

---

#### 3. GENERAR test-strategy.md

**Archivo:** `plans/cs-gestionar-necesidades/frontend-admin/test-strategy.md`

**Contenido minimo:**

```markdown
# Test Strategy: Gestionar Necesidades (Admin Dashboard)

## 1. Cobertura de Tests

| Tipo | Objetivo | Cantidad |
|------|----------|----------|
| Unit (schemas Zod) | 100% coverage de refines | 8 tests |
| Unit (componentes) | Renderizado condicional + ARIA | 12 tests |
| Unit (hooks) | Logica de negocio + cache | 6 tests |
| Integration (forms) | Submit + validacion + errores | 5 tests |
| Integration (mutations) | Invalidation cache + toasts | 4 tests |
| E2E | Flujos completos usuario | 3 tests |
| **TOTAL** | **38 tests** | |

## 2. Unit Tests - Schemas Zod

**Archivo:** `src/shared/schemas/__tests__/crowdsourcing.schema.test.ts`

### createNecesidadSchema

1. Test: Rechaza titulo vacio
2. Test: Rechaza titulo < 5 chars
3. Test: Acepta titulo valido (5-200 chars)
4. Test: Rechaza descripcion > 4000 chars
5. Test: Rechaza presupuestoMax < presupuestoMin
6. Test: Rechaza presupuesto sin monedaId
7. Test: Rechaza modalidad Presencial sin ubicacion
8. Test: Rechaza fechaLimitePropuestas < hoy

## 3. Unit Tests - Componentes

**Archivo:** `src/admin/src/features/crowdsourcing/necesidades/components/__tests__/`

### NecesidadCard.test.tsx

1. Test: Renderiza titulo, tipo, modalidad, presupuesto
2. Test: Renderiza badge de estado con color correcto (Abierta=green)
3. Test: Renderiza contador de propuestas
4. Test: Muestra boton Editar solo si estado === Abierta
5. Test: Muestra boton Cerrar si estado === Abierta o En Progreso
6. Test: Oculta boton Editar si estado === Cerrada
7. Test: Llama onClick al hacer click en card
8. Test: ARIA label correcto para screen readers

### EstadoBadge.test.tsx

1. Test: Renderiza "Abierta" con clase bg-green-900/20
2. Test: Renderiza "Cerrada" con clase bg-gray-900/20
3. Test: ARIA label correcto

### CerrarNecesidadDialog.test.tsx

1. Test: Renderiza warning alert
2. Test: Permite escribir motivo (max 500 chars)
3. Test: Llama onSuccess tras cerrar exitosamente
4. Test: Muestra spinner al enviar

## 4. Integration Tests - Forms

**Archivo:** `src/admin/src/features/crowdsourcing/necesidades/pages/__tests__/NuevaNecesidadPage.test.tsx`

1. Test: Renderiza formulario con campos vacios
2. Test: Muestra errores de validacion al enviar vacio
3. Test: Oculta ubicacion si modalidad === Remoto
4. Test: Muestra ubicacion si modalidad === Presencial
5. Test: Crea necesidad exitosamente y redirige a listado

## 5. Integration Tests - Mutations

**Archivo:** `src/admin/src/features/crowdsourcing/necesidades/hooks/__tests__/useCreateNecesidad.test.ts`

1. Test: Invalida cache de mis-necesidades tras crear
2. Test: Muestra toast success tras crear
3. Test: Muestra toast error si API retorna 400
4. Test: Redirige a listado tras crear

## 6. E2E Tests

**Archivo:** `e2e/crowdsourcing/gestionar-necesidades.spec.ts`

### Flujo completo: Crear necesidad

1. Login como artista
2. Navegar a /crowdsourcing/necesidades
3. Click "Nueva Necesidad"
4. Rellenar formulario con datos validos
5. Submit
6. Verificar toast success
7. Verificar necesidad aparece en listado

### Flujo completo: Editar necesidad

1. Login como artista
2. Abrir necesidad Abierta
3. Click "Editar"
4. Modificar titulo
5. Guardar
6. Verificar toast success
7. Verificar cambio reflejado en detalle

### Flujo completo: Cerrar necesidad con propuestas

1. Login como artista
2. Crear necesidad (o usar existente con propuestas)
3. Click "Cerrar"
4. Confirmar en dialogo
5. Verificar toast "X propuestas rechazadas"
6. Verificar estado cambia a "Cerrada"
7. Verificar boton Editar desaparece

## 7. Comandos de Testing

```bash
# Unit tests (Vitest)
cd src/admin
npm run test:unit

# Integration tests (Vitest + React Testing Library)
npm run test:integration

# E2E tests (Playwright)
npm run test:e2e

# Coverage report
npm run test:coverage
```

## 8. Objetivos de Cobertura

- **Schemas Zod:** 100% (critico)
- **Componentes:** 80% (lineas)
- **Hooks:** 90% (logica de negocio)
- **Overall:** 85%
```

**Cambio:** Definir suite completa de tests con nombres y casos especificos

---

### 9.2 Acciones Sugeridas (Mayor - Mejoran calidad)

#### 4. Documentar integracion con templates en frontend-plan

**Seccion a agregar en frontend-plan.md:**

```markdown
## 5. Integracion con Templates (US-CS-01)

**Flujo:**
1. Artista completa wizard de templates y selecciona necesidades
2. Wizard redirige a `/crowdsourcing/necesidades/nueva?fromTemplate={templateId}`
3. NuevaNecesidadPage detecta query param
4. Fetch template data (useTemplateById)
5. Pre-rellena form con datos de template (titulo, descripcion, tipo, modalidad, presupuesto)
6. Artista puede editar campos antes de submit
7. Submit a POST /api/crowdsourcing/necesidades (mismo endpoint que creacion manual)

**Codigo:**

```typescript
// NuevaNecesidadPage.tsx
const [searchParams] = useSearchParams();
const fromTemplateId = searchParams.get('fromTemplate');
const { data: template } = useTemplateById(fromTemplateId, { enabled: !!fromTemplateId });

useEffect(() => {
  if (template) {
    form.reset({
      titulo: template.titulo,
      descripcion: template.descripcion,
      tipoNecesidadId: template.tipoNecesidadId,
      modalidadTrabajoId: template.modalidadTrabajoId,
      presupuestoMin: template.presupuestoMin,
      presupuestoMax: template.presupuestoMax,
      monedaId: template.monedaId,
      // proyectoArtisticoId lo selecciona el usuario manualmente
    });
  }
}, [template]);
```
```

#### 5. Especificar logica condicional de botones en ui-design

**Seccion a agregar en ui-design.md:**

```markdown
## Logica Condicional de Botones

### NecesidadCard - Boton Editar

**Visible si:**
- `necesidad.estadoNecesidadId === ESTADO_NECESIDAD.ABIERTA` (1)

**Oculto si:**
- `necesidad.estadoNecesidadId !== ESTADO_NECESIDAD.ABIERTA`

**Tooltip (si disabled):**
- "Solo se pueden editar necesidades en estado Abierta"

**Codigo:**
```tsx
{necesidad.estadoNecesidadId === ESTADO_NECESIDAD.ABIERTA && (
  <Button variant="ghost" size="sm" onClick={handleEdit}>
    Editar
  </Button>
)}
```

### NecesidadCard - Boton Cerrar

**Visible si:**
- `necesidad.estadoNecesidadId === ESTADO_NECESIDAD.ABIERTA` (1) O
- `necesidad.estadoNecesidadId === ESTADO_NECESIDAD.EN_PROGRESO` (2)

**Oculto si:**
- `necesidad.estadoNecesidadId === ESTADO_NECESIDAD.CERRADA` (3) O
- `necesidad.estadoNecesidadId === ESTADO_NECESIDAD.CANCELADA` (4)

**Codigo:**
```tsx
{(necesidad.estadoNecesidadId === ESTADO_NECESIDAD.ABIERTA ||
  necesidad.estadoNecesidadId === ESTADO_NECESIDAD.EN_PROGRESO) && (
  <Button variant="ghost" size="sm" className="text-red-400" onClick={handleCerrar}>
    Cerrar
  </Button>
)}
```

### EditarNecesidadPage - Manejo de error 400

**Si API retorna 400 con errorCode 4001:**
1. Mostrar toast error: "Solo se pueden editar necesidades en estado Abierta"
2. Redirect a `/crowdsourcing/necesidades` (listado)
3. No mostrar formulario de edicion

**Codigo:**
```tsx
const { data: necesidad, error } = useNecesidadById(id);

if (error?.response?.status === 400) {
  toast({ title: 'Solo se pueden editar necesidades en estado Abierta', variant: 'destructive' });
  navigate(APP_ROUTES.dashboard.crowdsourcing.necesidades);
  return null;
}

if (necesidad && necesidad.estadoNecesidadId !== ESTADO_NECESIDAD.ABIERTA) {
  // Redirect preventivo en frontend
  navigate(APP_ROUTES.dashboard.crowdsourcing.necesidades);
  return null;
}
```
```

#### 6. Documentar invalidacion de cache en frontend-plan

**Seccion a agregar en frontend-plan.md:**

```markdown
## 8. Invalidacion de Cache

### Tras Crear Necesidad

```typescript
export function useCreateNecesidad() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateNecesidadRequest) => necesidadService.create(data),
    onSuccess: () => {
      // Invalidar listado de mis necesidades
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.necesidades.mis });

      // Opcional: Pre-fetch primera pagina para UX instantaneo
      queryClient.prefetchQuery({
        queryKey: QUERY_KEYS.crowdsourcing.necesidades.mis.concat([{ page: 1 }]),
        queryFn: () => necesidadService.getMisNecesidades({ page: 1 }),
      });
    },
  });
}
```

### Tras Editar Necesidad

```typescript
export function useUpdateNecesidad(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: UpdateNecesidadRequest) => necesidadService.update(id, data),
    onSuccess: () => {
      // Invalidar detalle de esta necesidad especifica
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.necesidades.byId(id) });

      // Invalidar listado (titulo/descripcion pueden haber cambiado)
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.necesidades.mis });
    },
  });
}
```

### Tras Cerrar Necesidad

```typescript
export function useCerrarNecesidad(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CerrarNecesidadRequest) => necesidadService.cerrar(id, data),
    onSuccess: (result) => {
      // Invalidar detalle (estado cambio + propuestas rechazadas)
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.necesidades.byId(id) });

      // Invalidar listado (badge estado cambio)
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.necesidades.mis });

      // Toast con contador de propuestas rechazadas
      toast({
        title: `Necesidad cerrada. ${result.data.propuestasRechazadas} propuestas rechazadas.`,
        variant: 'success',
      });
    },
  });
}
```
```

### 9.3 Nice to Have (Menor - Mejoras UX)

#### 7. Agregar badge urgente para fecha limite proxima

**Seccion a agregar en ui-design.md:**

```markdown
### Fecha Limite - Badge Urgente

**Reglas:**
- Si `fechaLimitePropuestas` < hoy + 3 dias → Badge rojo "URGENTE"
- Si `fechaLimitePropuestas` < hoy + 7 dias → Badge amarillo "Cierra pronto"
- Si `fechaLimitePropuestas` >= hoy + 7 dias → Sin badge especial

**Codigo:**
```tsx
const diasRestantes = differenceInDays(new Date(necesidad.fechaLimitePropuestas), new Date());

{necesidad.fechaLimitePropuestas && (
  <div className="flex items-center gap-2 mb-4">
    <Calendar className="w-4 h-4 text-[#64748b]" />
    <span className="text-sm text-[#94a3b8]">
      Límite: {format(new Date(necesidad.fechaLimitePropuestas), 'dd MMM yyyy')}
    </span>
    {diasRestantes < 3 && (
      <Badge variant="destructive" size="sm" aria-label="Fecha límite urgente">
        URGENTE - {diasRestantes} días
      </Badge>
    )}
    {diasRestantes >= 3 && diasRestantes < 7 && (
      <Badge variant="warning" size="sm" aria-label="Fecha límite próxima">
        Cierra pronto
      </Badge>
    )}
  </div>
)}
```
```

#### 8. Especificar ordenamiento de propuestas en detalle

**Seccion a agregar en ui-design.md:**

```markdown
### PropuestaCard - Ordenamiento

**Orden por defecto:**
- `propuestas.sort((a, b) => new Date(b.fechaCreacion) - new Date(a.fechaCreacion))`
- Mas recientes primero (DESC)

**Futuro (opcional):**
- Permitir ordenar por precio (ASC/DESC)
- Permitir ordenar por rating del profesional (DESC)

**Codigo:**
```tsx
const propuestasOrdenadas = useMemo(() => {
  return [...necesidad.propuestas].sort((a, b) =>
    new Date(b.fechaCreacion).getTime() - new Date(a.fechaCreacion).getTime()
  );
}, [necesidad.propuestas]);
```
```

---

## 10. Checklist de Validacion QA

### Pre-implementacion
- [ ] Revisar que contracts.md esta alineado con feature-spec.md (COMPLETO)
- [ ] Confirmar que ui-ux.md cubre todos los AC de UI (COMPLETO)
- [ ] Verificar que backend ha definido maestras (TipoNecesidad, Modalidad, Moneda)
- [ ] Validar que ProyectoArtistico entity existe (dependencia)

### Generacion de Planes
- [x] contracts-plan.md generado (COMPLETO)
- [ ] **frontend-plan.md generado (CRITICO - FALTA)**
- [ ] **ui-design.md generado (CRITICO - FALTA)**
- [ ] **test-strategy.md generado (CRITICO - FALTA)**

### Durante implementacion
- [ ] Types compartidos creados en `src/shared/types/crowdsourcing.ts`
- [ ] Schemas Zod con refines en `src/shared/schemas/crowdsourcing.schema.ts`
- [ ] Constantes en `src/shared/constants/index.ts` (QUERY_KEYS, API_ROUTES, estados, etc)
- [ ] Formatter `formatPresupuesto` en `src/shared/utils/format.ts`
- [ ] Error messages en `src/shared/utils/error-messages.ts`
- [ ] Componentes en `src/admin/src/features/crowdsourcing/necesidades/components/`
- [ ] Hooks en `src/admin/src/features/crowdsourcing/necesidades/hooks/`
- [ ] Pages en `src/admin/src/features/crowdsourcing/necesidades/pages/`
- [ ] Service en `src/admin/src/features/crowdsourcing/necesidades/services/`
- [ ] Routing en `src/admin/src/app/routes.tsx`

### Testing
- [ ] Unit tests de schemas Zod (8 tests)
- [ ] Unit tests de componentes (12 tests)
- [ ] Unit tests de hooks (6 tests)
- [ ] Integration tests de forms (5 tests)
- [ ] Integration tests de mutations (4 tests)
- [ ] E2E tests de flujos (3 tests)
- [ ] Coverage >= 85%

### Post-implementacion
- [ ] Validar que crear necesidad funciona end-to-end
- [ ] Validar que editar solo Abierta funciona (boton oculto + error 400)
- [ ] Validar que cerrar necesidad rechaza propuestas y muestra contador
- [ ] Validar que filtros (estado + search) funcionan
- [ ] Validar que listado paginado carga correctamente
- [ ] Validar que integracion con templates (query param) funciona
- [ ] Validar que badges de estado tienen colores correctos
- [ ] Validar que validaciones Zod funcionan en tiempo real
- [ ] Validar que toast messages son correctos
- [ ] Validar responsive mobile (1 col) y desktop (2-3 cols)
- [ ] Validar accesibilidad (ARIA labels, keyboard nav, focus states)

---

## 11. Conclusion

**Score Final:** 50% (Contratos 100% completos, Implementacion Frontend 0%)

**Veredicto:** REQUIERE GENERACION DE PLANES FRONTEND

**Gaps Criticos Identificados:**
1. **frontend-plan.md** - NO EXISTE (bloquea desarrollo)
2. **ui-design.md** - NO EXISTE (sin especificacion de componentes)
3. **test-strategy.md** - NO EXISTE (sin estrategia de testing)

**Proximo Paso:**

**ACCION INMEDIATA REQUERIDA:**
1. Generar `plans/cs-gestionar-necesidades/frontend-admin/frontend-plan.md`
2. Generar `plans/cs-gestionar-necesidades/frontend-admin/ui-design.md`
3. Generar `plans/cs-gestionar-necesidades/frontend-admin/test-strategy.md`

**Una vez generados los 3 planes faltantes:**
- Re-ejecutar validacion QA para verificar cobertura 100%
- Proceder con implementacion backend (paralelo)
- Proceder con implementacion frontend Admin (requiere contratos shared completos)

**Fortalezas del trabajo actual:**
- Contratos (types, schemas, constants) exhaustivos y bien documentados
- UI/UX especificado con gran detalle (design tokens, responsive, accesibilidad)
- Validaciones compartidas backend/frontend alineadas
- Error handling bien documentado
- Integracion con templates (US-CS-01) considerada

**Debilidades criticas:**
- Sin plan de arquitectura frontend (componentes, hooks, services)
- Sin especificacion de componentes React concretos
- Sin estrategia de testing con casos especificos

**Recomendacion final:**
No iniciar desarrollo frontend sin los 3 planes faltantes. La implementacion sin plan resultara en:
- Arquitectura inconsistente
- Componentes no reutilizables
- Tests insuficientes o incorrectos
- Re-trabajo y bugs

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-16
**Proxima revision:** Tras generar frontend-plan.md, ui-design.md, test-strategy.md
