# Sistema de Calculo de Costos de Transporte

Aplicacion de escritorio en **C# / .NET Framework 4.7.2 / Windows Forms** para calcular el **costo de operar un vehiculo por kilometraje** y el **precio recomendado de un servicio de transporte**, con persistencia local en **SQLite**.

---

## 1. Arquitectura del proyecto

```
Proyecto/
├── Proyecto.csproj              (proyecto estilo SDK, .NET Framework 4.7.2, SQLite)
├── Program.cs                   (punto de entrada: inicializa BD y abre FormPrincipal)
├── App.config                   (configuracion de runtime)
│
├── Models/                      (clases y enumeraciones del dominio)
│   ├── Enums.cs                 (TipoVehiculo, TipoServicio, TipoCosto, Periodicidad...)
│   ├── Vehiculo.cs
│   ├── Conductor.cs
│   ├── CostoVehiculo.cs
│   ├── ServicioTransporte.cs
│   ├── ResultadoCalculo.cs
│   └── ConfiguracionGlobal.cs
│
├── Data/                        (acceso a datos y persistencia)
│   ├── ConexionBD.cs            (conexion SQLite + helpers)
│   └── InicializadorBD.cs       (crea tablas y registra datos de ejemplo)
│
├── Calculators/
│   └── CalculadoraCostos.cs     (toda la logica de formulas de calculo)
│
├── Services/                    (CRUD sobre las tablas)
│   ├── VehiculoService.cs
│   ├── ConductorService.cs
│   ├── CostoService.cs
│   ├── ConfigService.cs
│   └── ServicioService.cs       (guarda/historial de calculos)
│
├── Utilities/
│   ├── Tablero.cs               (fabrica de controles con estilo)
│   ├── EnumeracionesUI.cs       (etiquetas en espanol de las enumeraciones)
│   ├── Formato.cs               (moneda RD$, kilometros, numeros)
│   └── Validaciones.cs          (reglas de validacion de negocio)
│
└── Forms/                       (interfaz grafica)
    ├── FormPrincipal.cs         (menu principal)
    ├── FormVehiculos.cs / FormVehiculoEditor.cs
    ├── FormConductores.cs / FormConductorEditor.cs
    ├── FormCostos.cs / FormCostoEditor.cs
    ├── FormCalculoCosto (FormCalculo.cs) / FormResultado.cs
    └── FormHistorial.cs
```

## 2. Formulas de calculo

Se separan los costos en **variables** (dependen de la distancia) y **fijos** (dependen del periodo).

### Costos variables (por km de uso)
```
CostoCombustiblePorKm = PrecioCombustible / Rendimiento   (precio por galon o litro, rendimiento en km/unidad)
CostoMantenimientoPorKm = monto registrado como "Variable (por km)"
PeajesVariosPorKm = monto registrado como "Variable (por km)" en la categoria Peajes
```

### Costos fijos (se convierten a mensual y se reparten entre el kilometraje mensual)
```
MontoMensual = Monto * factor     (Mensual = 1, Semanal = 4.333, Diario = 30)
CostoFijoPorKm = MontoMensual / KilometrajeMensualDelVehiculo
```
Aplicados al salario del conductor: `SalarioPorKm = SalarioMensual / KilometrajeMensual`

### Resumen del servicio
```
CombustibleUtilizado = Distancia / Rendimiento
CostoCombustible     = CombustiblePorKm x Distancia
CostoMantenimiento   = MantenimientoPorKm x Distancia
CostoSeguro          = SeguroPorKm x Distancia
CostoConductor       = SalarioPorKm x Distancia
CostoPeajes          = PeajesPorKm x Distancia + PeajesDeLaRuta(peaje manual del viaje)
CostoOtros           = OtrosPorKm x Distancia

CostoTotalPorKm      = Combustible + Mantenimiento + Seguro + Conductor + Peajes + Otros
CostoTotalServicio   = CostoTotalPorKm x Distancia + PeajesDeLaRuta

CostoPorPasajero     = CostoTotalServicio / Pasajeros   (solo si hay pasajeros)
CostoPorCargaKg      = CostoTotalServicio / CargaKg     (si hay carga)
CostoPorHora         = CostoTotalServicio / Horas       (si hay horas)

Ganancia             = CostoTotalServicio x (Margen / 100)
PrecioFinalRecomendado = CostoTotalServicio + Ganancia
```

**Ejemplo con datos reales de prueba** (vehiculo Toyota Corolla, precio galon RD$210, rendimiento 22 km/galon,
kilometraje mensual 2.200, seguro RD$8.200/mes, mantenimiento RD$1.20/km, salario conductor RD$30.000/mes,
distancia 50 km, 4 pasajeros, 150 RD$ de peajes, margen 20%):

```
Combustible /Km = 210 / 22 = 9.545          Seguro /Km     = 8.200 / 2200 = 3.727
Salario /Km     = 30.000 / 2200 = 13.636    Mant /Km       = 1.20
CostoPorKm      = 9.545 + 1.2 + 3.727 + 13.636 = 28.109
CostCombustible = 9.545 x 50 = 477.27        Combustible usado = 50/22 = 2.27 gal
CostSeguro      = 3.727 x 50 = 186.36        CostConductor = 13.636 x 50 = 681.82
CostMantenim    = 1.2 x 50 = 60.00            Peajes = 150.00
CostoTotal      = 28.109 x 50 + 150 = 1.555.45
Por pasajero    = 1.555.45 / 4 = 388.86
Ganancia 20%    = 311.09
PRECIO FINAL    = RD$ 1.866.54
```

## 3. Base de datos (SQLite)

El archivo `transporte.db` se genera automaticamente en la carpeta `Datos\` junto al ejecutable
(`bin\Debug\net472\Datos\`).
Primera ejecucion: crea las tablas y registra vehiculos, conductores y costos de ejemplo.

| Tabla | Campos principales |
|---|---|
| Vehiculos | Id, Placa, Marca, Modelo, Tipo, CapacidadPasajeros, CapacidadCargaKg, Rendimiento, UnidadCombustible, KilometrajeMensual |
| Conductores | Id, Documento, Nombre, Telefono, SalarioMensual |
| CostosVehiculo | Id, VehiculoId, Nombre, Categoria (Mantenimiento/Seguro/Peajes/Otros), Tipo (Fijo/Variable), Periodicidad, Monto |
| Configuracion | Clave, Valor (PrecioCombustible, UnidadCombustible) |
| Servicios | Id, VehiculoId, ConductorId, Fecha, TipoServicio, DistanciaKm, Pasajeros, CargaKg, Horas, PeajesManual, MargenGanancia |
| Resultados | Id, ServicioId, todos los costos calculados, Ganancia, PrecioFinal |

## 4. Validaciones

- Distancia > 0.
- Pasajeros >= 0 y no supera la capacidad del vehiculo.
- Carga >= 0 y no supera la capacidad de carga.
- Rendimiento del combustible > 0.
- Camps obligatorios (placa, marca, modelo, nombre, documento, ...).
- Precios/montos no negativos.
- Un costo "Fijo" no puede tener periodicidad "Por km", y un costo "Variable" debe ser "Por km".
- Mensajes de error claros al usuario.

## 5. Como ejecutar el proyecto

Con **Visual Studio 2022/2025 y .NET Framework 4.7.2**:
1. Abra el archivo `Proyecto\Proyecto.csproj`.
2. Visual Studio restaura el paquete `System.Data.SQLite.Core` (necesita internet la primera vez).
3. Ejecute (F5). En la carpeta `Datos\` se crea la BD con datos de ejemplo.

Tambien puede compilar desde consola:
```
dotnet build
dotnet run --project Proyecto
```
(Ejecutable generado en `bin\Debug\net472\`.)

## 6. Uso del programa

1. **CALCULAR SERVICIO**: seleccione vehiculo, conductor, tipo de servicio, distancia, pasajeros, carga, horas, peajes y margen. Pulsa "Calcular Servicio".
2. **ADMINISTRAR VEHICULOS / CONDUCTORES / COSTOS**: alta/baja/modificacion/consulta.
3. **HISTORIAL DE RESULTADOS**: consulta y eliminacion de calculos anteriores (guardados en SQLite).

Moneda mostrada en Pesos Dominicanos (RD$). Los costos fijos se reparten por km usando el
kilometraje mensual del vehiculo; los variables dependen de la distancia recorrida.