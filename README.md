## Proyecto Sistema de Gestión de Taller Mecánico

    Sistema web desarrollado en MVC.NET que permite gestionar la recepción de clientes, sus vehículos, los pedidos de reparación, los servicios realizados, los repuestos utilizados y los movimientos de cada vehículo dentro del taller. Además, incluye auditorías automáticas para controlar las acciones realizadas por los usuarios del sistema.

## Entidades:
*   Usuario: Persona que utiliza el sistema. Posee un rol determinado (Administrador, Recepcionista o Mecánico). 
*   Administrador: Tiene control total sobre el sistema. Puede crear, modificar y eliminar usuarios, dar de baja entidades, y consultar el historial de acciones mediante auditorías. 
*   Recepcionista: Gestiona clientes, vehículos y pedidos. Registra los pagos o cancelaciones solicitadas por los clientes. 
*   Mecánico: Registra los servicios realizados a cada pedido y su finalización. 
*   Cliente: Persona que no usa el sistema directamente, pero cuyas acciones son registradas por el recepcionista. Es propietario de uno o más vehículos y puede solicitar pedidos, ingresar y retirar vehículos, y efectuar pagos. 
*   Vehículo: Propiedad de un cliente. Puede tener varios pedidos asociados.
*   Pedido: Solicitud de trabajo realizada por un cliente sobre un vehículo. Contiene las observaciones del cliente, los servicios a realizar, los costos estimados y finales, y su estado (Pendiente, En proceso, Finalizado, Pagado o Cancelado). 
*   Servicio: Trabajo o tarea realizada por un mecánico dentro de un pedido. Puede incluir varios tipos de servicio y repuestos asociados. TipoServicio: Catálogo de posibles servicios que ofrece el taller (cambio de aceite, pintura, alineación, etc.) con un costo base. 
*   DetalleTipoServicio: Asociación entre un servicio y sus tipos, con costos adicionales o motivos específicos. 
*   Repuesto: Elementos o materiales utilizados para reparar o reemplazar partes del vehículo, con un costo base. 
*   DetalleRepuesto: Relación entre un servicio y los repuestos empleados, con su cantidad correspondiente.

## Funcionalidades:
# 👤 Administración de Usuarios 
    Crear, editar, eliminar y desactivar usuarios del sistema. 
    Asignar roles (Administrador, Recepcionista, Mecánico). 
    Consultar el historial de acciones mediante auditorías. 

# 👥 Gestión de Clientes 
    Registrar nuevos clientes con sus datos personales. 
    Editar o dar de baja clientes. 
    Consultar el historial de pedidos o deudas del cliente. 

# 🚗 Gestión de Vehículos 
    Registrar vehículos asociados a un cliente. 
    Consultar si un vehículo está en el taller. 
    Ver historial de pedidos y movimientos del vehículo. 

# 📋 Gestión de Pedidos 
    Crear pedidos nuevos asociados a un vehículo y cliente.
    Cambiar estado del pedido (pendiente, en proceso, finalizado, etc.). 
    Registrar pago o cancelación del pedido. 
    Generar informes de pedidos por estado o por fecha. 

# 🔧 Gestión de Servicios 
    Registrar servicios realizados por un mecánico. 
    Asociar tipos de servicio y repuestos utilizados. 
    Calcular automáticamente el costo estimado y final del servicio. 
    Finalizar servicios o reabrirlos en caso de ajustes.

# 🧾 Gestión de Repuestos 
    Registrar repuestos disponibles. 
    Asociarlos a servicios realizados. Controlar el consumo de repuestos en el taller. 
