'use strict';

// ── CONFIG ────────────────────────────────────────────────────
const API = 'http://localhost:5000/api';

// ── PERMISOS POR ROL ──────────────────────────────────────────
/*
  Aquí se define TODO lo que cada rol puede hacer.
  Si quieres cambiar permisos, solo cambias este objeto.

  canView:   qué secciones ve en el menú
  canCreate: en qué secciones puede crear registros
  canEdit:   en qué secciones puede editar
  canDelete: en qué secciones puede eliminar
*/
const ROLES = {
  Administrador: {
    color:     '#e8ff47',
    colorVar:  'var(--rol-admin)',
    badgeClass:'admin',
    label:     'Administrador',
    canView:   ['dashboard','hoteles','habitaciones','clientes','empleados','reservaciones','usuarios'],
    canCreate: ['hoteles','habitaciones','clientes','empleados','reservaciones','usuarios'],
    canEdit:   ['hoteles','habitaciones','clientes','empleados','reservaciones','usuarios'],
    canDelete: ['hoteles','habitaciones','clientes','empleados','reservaciones','usuarios'],
    dashboard: 'full',       // ve todas las stats
  },
  Supervisor: {
    color:     '#47b8ff',
    colorVar:  'var(--rol-supervisor)',
    badgeClass:'supervisor',
    label:     'Supervisor',
    canView:   ['dashboard','reportes','habitaciones','reservaciones','clientes'],
    canCreate: [],           // no puede crear nada
    canEdit:   ['habitaciones'], // solo puede cambiar estado de habitación
    canDelete: [],           // no puede eliminar nada
    dashboard: 'reportes',  // ve dashboard de reportes
  },
  Recepcionista: {
    color:     '#44ff88',
    colorVar:  'var(--rol-recepcion)',
    badgeClass:'recepcion',
    label:     'Recepcionista',
    canView:   ['dashboard','reservaciones','clientes','habitaciones'],
    canCreate: ['reservaciones','clientes'],
    canEdit:   ['reservaciones','clientes'],
    canDelete: [],           // no puede eliminar nada
    dashboard: 'recepcion', // ve dashboard de recepción
  },
};

// CACHE 
const cache = { hoteles:[], tipos:[], clientes:[], empleados:[], habitaciones:[], reservaciones:[] };
let modalState = { type: null, editId: null };

//FETCH
async function api(path, method = 'GET', body = null) {
  const opts = { method, headers: { 'Content-Type': 'application/json' } };
  if (body) opts.body = JSON.stringify(body);
  const res = await fetch(API + path, opts);
  if (!res.ok) throw new Error(await res.text() || `HTTP ${res.status}`);
  const ct = res.headers.get('content-type') || '';
  return ct.includes('json') ? res.json() : res.text();
}

// AUTH
function getUser()   { try { return JSON.parse(localStorage.getItem('_hu')); } catch { return null; } }
function setUser(u)  { localStorage.setItem('_hu', JSON.stringify(u)); }
function clearUser() { localStorage.removeItem('_hu'); }

// Retorna el objeto de permisos del usuario actual
function getRol() {
  const user = getUser();
  return ROLES[user?.rol] || ROLES['Recepcionista'];
}

// ¿Tiene permiso para hacer esta acción en esta sección?
function can(action, section) {
  const rol = getRol();
  const key = `can${action.charAt(0).toUpperCase() + action.slice(1)}`;
  return rol[key]?.includes(section) ?? false;
}

async function doLogin() {
  const username = document.getElementById('lUser').value.trim();
  const password = document.getElementById('lPass').value.trim();
  const btn = document.getElementById('lBtn');
  const err = document.getElementById('lErr');
  if (!username || !password) return;

  btn.textContent = '...'; btn.disabled = true;
  err.style.display = 'none';

  try {
    const user = await api('/usuario/login', 'POST', { username, password });
    setUser(user);
    bootApp(user);
  } catch {
    err.style.display = 'block';
    btn.textContent = 'Entrar →'; btn.disabled = false;
  }
}

function doLogout() {
  clearUser();
  Object.keys(cache).forEach(k => cache[k] = []);
  document.getElementById('root').style.display = 'none';
  const ls = document.getElementById('loginScreen');
  ls.classList.remove('hide'); ls.style.display = 'flex';
  document.getElementById('lUser').value = '';
  document.getElementById('lPass').value = '';
  // Resetear color de rol
  document.documentElement.style.setProperty('--rol-color', '#e8ff47');
}

// Rellena el login con las credenciales de prueba al hacer clic
function fillLogin(username, password) {
  document.getElementById('lUser').value = username;
  document.getElementById('lPass').value = password;
}

function bootApp(user) {
  const rol = ROLES[user.rol] || ROLES['Recepcionista'];

  // Aplicar color del rol como variable CSS global
  document.documentElement.style.setProperty('--rol-color', rol.color);

  document.getElementById('loginScreen').classList.add('hide');
  setTimeout(() => {
    document.getElementById('loginScreen').style.display = 'none';
    document.getElementById('root').style.display        = 'flex';

    // Actualizar sidebar con datos del usuario
    document.getElementById('userNameDisplay').textContent = user.username;
    document.getElementById('avatarLetter').textContent    = user.username[0].toUpperCase();
    document.getElementById('userRoleDisplay').textContent = rol.label;
    document.getElementById('rolBadge').className          = `rol-badge ${rol.badgeClass}`;
    document.getElementById('rolBadge').textContent        = rol.label;

    // Construir el menú según el rol
    buildNav(rol);

    // Ir al dashboard
    navigate('dashboard');
  }, 380);
}

// ── NAVEGACIÓN ────────────────────────────────────────────────
const PAGE_TITLES = {
  dashboard:    'Dashboard',
  hoteles:      'Hoteles',
  habitaciones: 'Habitaciones',
  clientes:     'Clientes',
  empleados:    'Empleados',
  reservaciones:'Reservaciones',
  usuarios:     'Usuarios',
  reportes:     'Reportes',
};

const PAGE_LOADERS = {
  dashboard:    loadDashboard,
  hoteles:      loadHoteles,
  habitaciones: loadHabitaciones,
  clientes:     loadClientes,
  empleados:    loadEmpleados,
  reservaciones:loadReservaciones,
  usuarios:     loadUsuarios,
  reportes:     loadReportes,
};

// Construye el menú lateral según el rol del usuario
function buildNav(rol) {
  const nav = document.getElementById('navItems');

  // Definición completa del menú con secciones
  const menuGroups = [
    {
      items: [
        { id: 'dashboard', icon: '⊞', label: 'Dashboard' },
      ]
    },
    {
      title: 'Gestión',
      items: [
        { id: 'hoteles',       icon: '🏨', label: 'Hoteles' },
        { id: 'habitaciones',  icon: '🛏', label: 'Habitaciones' },
        { id: 'reservaciones', icon: '📋', label: 'Reservaciones' },
      ]
    },
    {
      title: 'Personas',
      items: [
        { id: 'clientes',  icon: '👤', label: 'Clientes' },
        { id: 'empleados', icon: '👔', label: 'Empleados' },
      ]
    },
    {
      title: 'Sistema',
      items: [
        { id: 'reportes', icon: '📊', label: 'Reportes' },
        { id: 'usuarios', icon: '🔑', label: 'Usuarios' },
      ]
    },
  ];

  nav.innerHTML = menuGroups.map(group => {
    const visibleItems = group.items.filter(item => rol.canView.includes(item.id));
    if (!visibleItems.length) return '';

    const titleHtml = group.title ? `<div class="nav-section">${group.title}</div>` : '';
    const itemsHtml = visibleItems.map(item => `
      <button class="nav-btn" data-view="${item.id}" onclick="navigate('${item.id}')">
        <span class="nav-icon">${item.icon}</span> ${item.label}
      </button>`).join('');

    return titleHtml + itemsHtml + `<div class="nav-divider"></div>`;
  }).join('');
}

function navigate(page) {
  const rol = getRol();
  if (!rol.canView.includes(page)) {
    toast('Sin acceso a esta sección', 'warn'); return;
  }

  document.querySelectorAll('.view').forEach(v => v.classList.remove('active'));
  document.querySelectorAll('.nav-btn').forEach(b => b.classList.remove('active'));

  document.getElementById('view-' + page)?.classList.add('active');
  document.querySelector(`[data-view="${page}"]`)?.classList.add('active');
  document.getElementById('pageTitle').innerHTML =
    `${PAGE_TITLES[page]} <span id="pageCount"></span>`;

  PAGE_LOADERS[page]?.();
}

function setCount(n) {
  const el = document.getElementById('pageCount');
  if (el) el.textContent = `· ${n} registros`;
}

// ── TOAST ─────────────────────────────────────────────────────
function toast(msg, type = 'ok') {
  const el = document.createElement('div');
  el.className = 'toast';
  el.innerHTML = `<div class="toast-dot ${type}"></div><span>${msg}</span>`;
  document.getElementById('toastStack').prepend(el);
  setTimeout(() => { el.classList.add('out'); setTimeout(() => el.remove(), 300); }, 3000);
}

// ── HELPERS ───────────────────────────────────────────────────
const byHotel  = id => cache.hoteles.find(x => x.idHotel === id)?.nombre || `#${id}`;
const byTipo   = id => cache.tipos.find(x => x.idTipo === id)?.nombre    || `#${id}`;
const byClient = id => { const c = cache.clientes.find(x => x.idCliente === id);  return c ? `${c.nombre} ${c.apellido}` : `#${id}`; };
const byEmpl   = id => { const e = cache.empleados.find(x => x.idEmpleado === id); return e ? `${e.nombre} ${e.apellido}` : `#${id}`; };
const esc      = s => (s || '').replace(/\\/g,'\\\\').replace(/'/g,"\\'").replace(/"/g,'\\"');
const v        = name => { const el = document.getElementById('f_' + name); return el ? el.value.trim() : ''; };

function emptyRow(icon, msg, cols) {
  return `<tr><td colspan="${cols}"><div class="empty"><div class="empty-icon">${icon}</div><p>${msg}</p></div></td></tr>`;
}

// Genera botones de acción respetando permisos del rol
function actionBtns(section, editFn, deleteFn) {
  const btns = [];
  if (can('edit', section))   btns.push(`<button class="btn btn-ghost btn-sm" onclick="${editFn}">editar</button>`);
  if (can('delete', section)) btns.push(`<button class="btn btn-danger btn-sm" onclick="${deleteFn}">eliminar</button>`);
  if (!btns.length) return `<span style="font-size:11px;color:var(--muted)">solo lectura</span>`;
  return `<div class="td-actions">${btns.join('')}</div>`;
}

// ── DASHBOARD ─────────────────────────────────────────────────
async function loadDashboard() {
  const rol = getRol();
  // Mostrar el dashboard correcto según el rol
  document.querySelectorAll('.dash-view').forEach(d => d.style.display = 'none');
  document.getElementById(`dash-${rol.dashboard}`)?.style.setProperty('display', 'block');

  if (rol.dashboard === 'full')      await loadDashFull();
  if (rol.dashboard === 'reportes')  await loadDashReportes();
  if (rol.dashboard === 'recepcion') await loadDashRecepcion();
}

async function loadDashFull() {
  try {
    const [hoteles, habitaciones, clientes, empleados, reservaciones, tipos] = await Promise.all([
      api('/hotel'), api('/habitacion'), api('/cliente'),
      api('/empleado'), api('/reservacion'), api('/tipohabitacion'),
    ]);
    Object.assign(cache, { hoteles, habitaciones, clientes, empleados, tipos, reservaciones });

    document.getElementById('s-hoteles').textContent       = hoteles.length;
    document.getElementById('s-habitaciones').textContent  = habitaciones.length;
    document.getElementById('s-clientes').textContent      = clientes.length;
    document.getElementById('s-empleados').textContent     = empleados.length;
    document.getElementById('s-reservaciones').textContent = reservaciones.length;

    const dr = document.getElementById('dash-reserv-list');
    const recientes = [...reservaciones].reverse().slice(0, 5);
    dr.innerHTML = recientes.length === 0
      ? '<div class="empty"><p>Sin reservaciones aún</p></div>'
      : recientes.map(r => `
          <div class="dash-row">
            <div>
              <div class="dash-row-label">${byClient(r.idCliente)}</div>
              <div class="dash-row-meta">${r.fechaInicio} → ${r.fechaFin}</div>
            </div>
            <div class="dash-row-value">Q${Number(r.total||0).toFixed(2)}</div>
          </div>`).join('');

    const dh = document.getElementById('dash-hab-estado');
    const disp = habitaciones.filter(h => h.estado === 'Disponible').length;
    const ocup = habitaciones.filter(h => h.estado === 'Ocupada').length;
    const mant = habitaciones.filter(h => h.estado === 'Mantenimiento').length;
    dh.innerHTML = [
      { label:'Disponibles',   val:disp, cls:'badge-ok' },
      { label:'Ocupadas',      val:ocup, cls:'badge-ocu' },
      { label:'Mantenimiento', val:mant, cls:'badge-man' },
    ].map(x => `
      <div class="dash-row">
        <span class="dash-row-label">${x.label}</span>
        <span class="badge ${x.cls}">${x.val}</span>
      </div>`).join('');
  } catch(e) { toast('Error: ' + e.message, 'err'); }
}

async function loadDashReportes() {
  try {
    const [habitaciones, reservaciones, clientes] = await Promise.all([
      api('/habitacion'), api('/reservacion'), api('/cliente'),
    ]);
    Object.assign(cache, { habitaciones, reservaciones, clientes });

    const disp = habitaciones.filter(h => h.estado === 'Disponible').length;
    const ocup = habitaciones.filter(h => h.estado === 'Ocupada').length;
    const mant = habitaciones.filter(h => h.estado === 'Mantenimiento').length;
    const totalIngresos = reservaciones.reduce((sum, r) => sum + Number(r.total || 0), 0);
    const ocupPct = habitaciones.length ? Math.round((ocup / habitaciones.length) * 100) : 0;

    document.getElementById('r-disponibles').textContent   = disp;
    document.getElementById('r-ocupadas').textContent      = ocup;
    document.getElementById('r-mantenimiento').textContent = mant;
    document.getElementById('r-ocupacion').textContent     = `${ocupPct}%`;
    document.getElementById('r-reservaciones').textContent = reservaciones.length;
    document.getElementById('r-clientes').textContent      = clientes.length;
    document.getElementById('r-ingresos').textContent      = `Q${totalIngresos.toFixed(2)}`;
  } catch(e) { toast('Error: ' + e.message, 'err'); }
}

async function loadDashRecepcion() {
  try {
    const [habitaciones, reservaciones, clientes] = await Promise.all([
      api('/habitacion'), api('/reservacion'), api('/cliente'),
    ]);
    if (!cache.hoteles.length) cache.hoteles = await api('/hotel');
    Object.assign(cache, { habitaciones, reservaciones, clientes });

    document.getElementById('rc-disponibles').textContent  = habitaciones.filter(h => h.estado === 'Disponible').length;
    document.getElementById('rc-reservaciones').textContent= reservaciones.length;
    document.getElementById('rc-clientes').textContent     = clientes.length;

    // Últimas reservaciones del día
    const hoy = new Date().toISOString().split('T')[0];
    const recientes = [...reservaciones].reverse().slice(0, 5);
    const list = document.getElementById('rc-reserv-list');
    list.innerHTML = recientes.length === 0
      ? '<div class="empty"><p>Sin reservaciones recientes</p></div>'
      : recientes.map(r => `
          <div class="dash-row">
            <div>
              <div class="dash-row-label">${byClient(r.idCliente)}</div>
              <div class="dash-row-meta">${byHotel(r.idHotel)} · ${r.fechaInicio} → ${r.fechaFin}</div>
            </div>
            <div class="dash-row-value">Q${Number(r.total||0).toFixed(2)}</div>
          </div>`).join('');

    // Habitaciones disponibles
    const habList = document.getElementById('rc-hab-list');
    const disponibles = habitaciones.filter(h => h.estado === 'Disponible').slice(0, 5);
    habList.innerHTML = disponibles.length === 0
      ? '<div class="empty"><p>Sin habitaciones disponibles</p></div>'
      : disponibles.map(h => `
          <div class="dash-row">
            <span class="dash-row-label">Hab. ${h.numero}</span>
            <span class="badge badge-ok">Disponible</span>
          </div>`).join('');
  } catch(e) { toast('Error: ' + e.message, 'err'); }
}

// ── REPORTES (vista completa para Supervisor) ─────────────────
async function loadReportes() {
  try {
    const [habitaciones, reservaciones, clientes, empleados] = await Promise.all([
      api('/habitacion'), api('/reservacion'), api('/cliente'), api('/empleado'),
    ]);
    Object.assign(cache, { habitaciones, reservaciones, clientes, empleados });

    const disp = habitaciones.filter(h => h.estado === 'Disponible').length;
    const ocup = habitaciones.filter(h => h.estado === 'Ocupada').length;
    const mant = habitaciones.filter(h => h.estado === 'Mantenimiento').length;
    const total = habitaciones.length;
    const ingresos = reservaciones.reduce((s, r) => s + Number(r.total || 0), 0);

    document.getElementById('rp-content').innerHTML = `
      <div class="stats-row">
        <div class="stat-card">
          <div class="stat-label">Total Habitaciones</div>
          <div class="stat-value">${total}</div>
          <div class="stat-icon">🛏</div>
        </div>
        <div class="stat-card">
          <div class="stat-label">Ocupadas</div>
          <div class="stat-value">${ocup}</div>
          <div class="stat-icon">🔴</div>
        </div>
        <div class="stat-card">
          <div class="stat-label">Disponibles</div>
          <div class="stat-value">${disp}</div>
          <div class="stat-icon">🟢</div>
        </div>
        <div class="stat-card">
          <div class="stat-label">% Ocupación</div>
          <div class="stat-value">${total ? Math.round(ocup/total*100) : 0}%</div>
          <div class="stat-icon">📊</div>
        </div>
        <div class="stat-card">
          <div class="stat-label">Ingresos Totales</div>
          <div class="stat-value" style="font-size:20px">Q${ingresos.toFixed(0)}</div>
          <div class="stat-icon">💰</div>
        </div>
      </div>

      <div class="dash-grid">
        <div class="report-card">
          <div class="report-card-title">Resumen de Operaciones</div>
          ${[
            ['Total de reservaciones', reservaciones.length],
            ['Total de clientes',      clientes.length],
            ['Total de empleados',     empleados.length],
            ['Promedio por reservación', reservaciones.length ? `Q${(ingresos/reservaciones.length).toFixed(2)}` : 'Q0'],
            ['Habitaciones en mantenimiento', mant],
          ].map(([label, val]) => `
            <div class="report-row">
              <span class="report-row-label">${label}</span>
              <span class="report-row-value">${val}</span>
            </div>`).join('')}
        </div>

        <div class="report-card">
          <div class="report-card-title">Últimas 5 Reservaciones</div>
          ${[...reservaciones].reverse().slice(0,5).map(r => `
            <div class="report-row">
              <span class="report-row-label">${byClient(r.idCliente)}<br><span style="font-size:10px;color:var(--muted)">${r.fechaInicio} → ${r.fechaFin}</span></span>
              <span class="report-row-value">Q${Number(r.total||0).toFixed(2)}</span>
            </div>`).join('') || '<div class="empty"><p>Sin datos</p></div>'}
        </div>
      </div>`;
  } catch(e) { toast('Error: ' + e.message, 'err'); }
}

// ── HOTELES ───────────────────────────────────────────────────
async function loadHoteles() {
  try {
    const data = await api('/hotel'); cache.hoteles = data; setCount(data.length);
    const tb = document.getElementById('tb-hoteles');
    if (!data.length) { tb.innerHTML = emptyRow('🏨','Sin hoteles',5); return; }
    tb.innerHTML = data.map(h => `<tr>
      <td class="td-id">${h.idHotel}</td>
      <td class="td-name">${h.nombre}</td>
      <td class="td-muted">${h.direccion||'—'}</td>
      <td class="td-muted">${h.telefono||'—'}</td>
      <td>${actionBtns('hoteles',`editHotel(${h.idHotel})`,`delHotel(${h.idHotel})`)}</td>
    </tr>`).join('');
  } catch(e) { toast(e.message,'err'); }
}
function editHotel(id) { openModal('hotel', cache.hoteles.find(x=>x.idHotel===id)); }
async function delHotel(id) {
  if (!can('delete','hoteles')) { toast('Sin permiso','warn'); return; }
  if (!confirm('¿Eliminar hotel?')) return;
  try { await api(`/hotel/${id}`,'DELETE'); toast('Hotel eliminado'); loadHoteles(); }
  catch(e) { toast(e.message,'err'); }
}

// ── HABITACIONES ──────────────────────────────────────────────
async function loadHabitaciones() {
  try {
    if (!cache.hoteles.length) cache.hoteles = await api('/hotel');
    if (!cache.tipos.length)   cache.tipos   = await api('/tipohabitacion');
    const data = await api('/habitacion'); cache.habitaciones = data; setCount(data.length);
    const BADGE = { Disponible:'badge-ok', Ocupada:'badge-ocu', Mantenimiento:'badge-man' };
    const tb = document.getElementById('tb-habitaciones');
    if (!data.length) { tb.innerHTML = emptyRow('🛏','Sin habitaciones',6); return; }
    tb.innerHTML = data.map(h => `<tr>
      <td class="td-id">${h.idHabitacion}</td>
      <td class="td-name">${h.numero}</td>
      <td class="td-muted">${byHotel(h.idHotel)}</td>
      <td class="td-muted">${byTipo(h.idTipo)}</td>
      <td><span class="badge ${BADGE[h.estado]||''}">${h.estado}</span></td>
      <td>${actionBtns('habitaciones',`editHab(${h.idHabitacion})`,`delHab(${h.idHabitacion})`)}</td>
    </tr>`).join('');
  } catch(e) { toast(e.message,'err'); }
}
function editHab(id) { openModal('habitacion', cache.habitaciones.find(x=>x.idHabitacion===id)); }
async function delHab(id) {
  if (!can('delete','habitaciones')) { toast('Sin permiso','warn'); return; }
  if (!confirm('¿Eliminar habitación?')) return;
  try { await api(`/habitacion/${id}`,'DELETE'); toast('Habitación eliminada'); loadHabitaciones(); }
  catch(e) { toast(e.message,'err'); }
}

// ── CLIENTES ──────────────────────────────────────────────────
async function loadClientes() {
  try {
    const data = await api('/cliente'); cache.clientes = data; setCount(data.length);
    const tb = document.getElementById('tb-clientes');
    if (!data.length) { tb.innerHTML = emptyRow('👤','Sin clientes',6); return; }
    tb.innerHTML = data.map(c => `<tr>
      <td class="td-id">${c.idCliente}</td>
      <td class="td-name">${c.nombre} ${c.apellido}</td>
      <td class="td-muted">${c.dpi||'—'}</td>
      <td class="td-muted">${c.telefono||'—'}</td>
      <td class="td-muted">${c.correo||'—'}</td>
      <td>${actionBtns('clientes',`editCliente(${c.idCliente})`,`delCliente(${c.idCliente})`)}</td>
    </tr>`).join('');
  } catch(e) { toast(e.message,'err'); }
}
function editCliente(id) { openModal('cliente', cache.clientes.find(x=>x.idCliente===id)); }
async function delCliente(id) {
  if (!can('delete','clientes')) { toast('Sin permiso','warn'); return; }
  if (!confirm('¿Eliminar cliente?')) return;
  try { await api(`/cliente/${id}`,'DELETE'); toast('Cliente eliminado'); loadClientes(); }
  catch(e) { toast(e.message,'err'); }
}

// ── EMPLEADOS ─────────────────────────────────────────────────
async function loadEmpleados() {
  try {
    const data = await api('/empleado'); cache.empleados = data; setCount(data.length);
    const tb = document.getElementById('tb-empleados');
    if (!data.length) { tb.innerHTML = emptyRow('👔','Sin empleados',5); return; }
    tb.innerHTML = data.map(e => `<tr>
      <td class="td-id">${e.idEmpleado}</td>
      <td class="td-name">${e.nombre} ${e.apellido}</td>
      <td class="td-muted">${e.puesto||'—'}</td>
      <td class="td-muted">${e.telefono||'—'}</td>
      <td>${actionBtns('empleados',`editEmpleado(${e.idEmpleado})`,`delEmpleado(${e.idEmpleado})`)}</td>
    </tr>`).join('');
  } catch(e) { toast(e.message,'err'); }
}
function editEmpleado(id) { openModal('empleado', cache.empleados.find(x=>x.idEmpleado===id)); }
async function delEmpleado(id) {
  if (!can('delete','empleados')) { toast('Sin permiso','warn'); return; }
  if (!confirm('¿Eliminar empleado?')) return;
  try { await api(`/empleado/${id}`,'DELETE'); toast('Empleado eliminado'); loadEmpleados(); }
  catch(e) { toast(e.message,'err'); }
}

// ── RESERVACIONES ─────────────────────────────────────────────
async function loadReservaciones() {
  try {
    if (!cache.clientes.length)  cache.clientes  = await api('/cliente');
    if (!cache.hoteles.length)   cache.hoteles   = await api('/hotel');
    if (!cache.empleados.length) cache.empleados = await api('/empleado');
    const data = await api('/reservacion'); cache.reservaciones = data; setCount(data.length);
    const tb = document.getElementById('tb-reservaciones');
    if (!data.length) { tb.innerHTML = emptyRow('📋','Sin reservaciones',7); return; }
    tb.innerHTML = data.map(r => `<tr>
      <td class="td-id">${r.idReservacion}</td>
      <td class="td-name">${byClient(r.idCliente)}</td>
      <td class="td-muted">${byHotel(r.idHotel)}</td>
      <td class="td-muted">${r.fechaInicio}</td>
      <td class="td-muted">${r.fechaFin}</td>
      <td style="color:var(--rol-color);font-family:var(--font-head);font-weight:700">Q${Number(r.total||0).toFixed(2)}</td>
      <td>${actionBtns('reservaciones',`editReserv(${r.idReservacion})`,`delReserv(${r.idReservacion})`)}</td>
    </tr>`).join('');
  } catch(e) { toast(e.message,'err'); }
}
function editReserv(id) { openModal('reservacion', cache.reservaciones.find(x=>x.idReservacion===id)); }
async function delReserv(id) {
  if (!can('delete','reservaciones')) { toast('Sin permiso','warn'); return; }
  if (!confirm('¿Eliminar reservación?')) return;
  try { await api(`/reservacion/${id}`,'DELETE'); toast('Reservación eliminada'); loadReservaciones(); }
  catch(e) { toast(e.message,'err'); }
}

// ── USUARIOS (solo Admin) ─────────────────────────────────────
async function loadUsuarios() {
  if (!can('view','usuarios')) { showAccessDenied('usuarios'); return; }
  try {
    const data = await api('/usuario');
    setCount(data.length);
    const ROL_BADGE = {
      'Administrador':'badge-rol-admin',
      'Supervisor':'badge-rol-supervisor',
      'Recepcionista':'badge-rol-recepcion',
    };
    const tb = document.getElementById('tb-usuarios');
    if (!data.length) { tb.innerHTML = emptyRow('🔑','Sin usuarios',5); return; }
    tb.innerHTML = data.map(u => `<tr>
      <td class="td-id">${u.idUsuario}</td>
      <td class="td-name">${u.username}</td>
      <td><span class="badge ${ROL_BADGE[u.rol]||''}">${u.rol||'—'}</span></td>
      <td class="td-muted">${byEmpl(u.idEmpleado)}</td>
      <td>${actionBtns('usuarios',`editUsuario(${u.idUsuario})`,`delUsuario(${u.idUsuario})`)}</td>
    </tr>`).join('');
  } catch(e) { toast(e.message,'err'); }
}
function editUsuario(id) { /* implementar si la API lo permite */ toast('Edición de usuarios próximamente','warn'); }
async function delUsuario(id) {
  if (!can('delete','usuarios')) { toast('Sin permiso','warn'); return; }
  if (!confirm('¿Eliminar usuario?')) return;
  try { await api(`/usuario/${id}`,'DELETE'); toast('Usuario eliminado'); loadUsuarios(); }
  catch(e) { toast(e.message,'err'); }
}

function showAccessDenied(section) {
  const view = document.getElementById('view-' + section);
  if (view) view.innerHTML = `
    <div class="access-denied">
      <div class="access-denied-icon">🔒</div>
      <h3>Acceso restringido</h3>
      <p>Tu rol no tiene permiso para ver esta sección.</p>
    </div>`;
}

// ── MODAL CONFIG ──────────────────────────────────────────────
const MODAL_CONFIGS = {
  hotel: {
    title: d => d ? 'Editar hotel' : 'Nuevo hotel',
    section: 'hoteles',
    fields: d => `
      <div class="field"><label class="field-label">Nombre *</label>
        <input class="field-input" id="f_nombre" value="${esc(d?.nombre||'')}" placeholder="Hotel Grand"></div>
      <div class="field"><label class="field-label">Dirección</label>
        <input class="field-input" id="f_direccion" value="${esc(d?.direccion||'')}" placeholder="Zona 10"></div>
      <div class="field"><label class="field-label">Teléfono</label>
        <input class="field-input" id="f_telefono" value="${esc(d?.telefono||'')}" placeholder="2222-3333"></div>`,
    validate: () => { const n=v('nombre'); if(!n){toast('Nombre obligatorio','err');return null;} return{nombre:n,direccion:v('direccion'),telefono:v('telefono')}; },
    save: async (data,id) => { if(id) await api(`/hotel/${id}`,'PUT',{...data,idHotel:id}); else await api('/hotel','POST',data); loadHoteles(); },
  },
  habitacion: {
    title: d => d ? 'Editar habitación' : 'Nueva habitación',
    section: 'habitaciones',
    fields: async d => {
      if (!cache.hoteles.length) cache.hoteles = await api('/hotel');
      if (!cache.tipos.length)   cache.tipos   = await api('/tipohabitacion');
      const hOpts = cache.hoteles.map(h=>`<option value="${h.idHotel}" ${d?.idHotel===h.idHotel?'selected':''}>${h.nombre}</option>`).join('');
      const tOpts = cache.tipos.map(t=>`<option value="${t.idTipo}" ${d?.idTipo===t.idTipo?'selected':''}>${t.nombre} — Q${t.precio}</option>`).join('');
      return `
        <div class="field-row">
          <div class="field"><label class="field-label">Número *</label>
            <input class="field-input" id="f_numero" value="${esc(d?.numero||'')}" placeholder="101"></div>
          <div class="field"><label class="field-label">Estado</label>
            <select class="field-select" id="f_estado">
              <option ${d?.estado==='Disponible'?'selected':''}>Disponible</option>
              <option ${d?.estado==='Ocupada'?'selected':''}>Ocupada</option>
              <option ${d?.estado==='Mantenimiento'?'selected':''}>Mantenimiento</option>
            </select></div>
        </div>
        <div class="field"><label class="field-label">Hotel *</label>
          <select class="field-select" id="f_idHotel">${hOpts}</select></div>
        <div class="field"><label class="field-label">Tipo *</label>
          <select class="field-select" id="f_idTipo">${tOpts}</select></div>`;
    },
    validate: () => { const n=v('numero'); if(!n){toast('Número obligatorio','err');return null;} return{numero:n,estado:v('estado'),idHotel:parseInt(v('idHotel')),idTipo:parseInt(v('idTipo'))}; },
    save: async (data,id) => { if(id) await api(`/habitacion/${id}`,'PUT',{...data,idHabitacion:id}); else await api('/habitacion','POST',data); loadHabitaciones(); },
  },
  cliente: {
    title: d => d ? 'Editar cliente' : 'Nuevo cliente',
    section: 'clientes',
    fields: d => `
      <div class="field-row">
        <div class="field"><label class="field-label">Nombre *</label><input class="field-input" id="f_nombre" value="${esc(d?.nombre||'')}" placeholder="Juan"></div>
        <div class="field"><label class="field-label">Apellido *</label><input class="field-input" id="f_apellido" value="${esc(d?.apellido||'')}" placeholder="Pérez"></div>
      </div>
      <div class="field"><label class="field-label">DPI</label><input class="field-input" id="f_dpi" value="${esc(d?.dpi||'')}" placeholder="1234567890101"></div>
      <div class="field-row">
        <div class="field"><label class="field-label">Teléfono</label><input class="field-input" id="f_telefono" value="${esc(d?.telefono||'')}" placeholder="5555-0000"></div>
        <div class="field"><label class="field-label">Correo</label><input class="field-input" id="f_correo" value="${esc(d?.correo||'')}" placeholder="juan@mail.com"></div>
      </div>`,
    validate: () => { const n=v('nombre'),a=v('apellido'); if(!n||!a){toast('Nombre y apellido obligatorios','err');return null;} return{nombre:n,apellido:a,dpi:v('dpi'),telefono:v('telefono'),correo:v('correo')}; },
    save: async (data,id) => { if(id) await api(`/cliente/${id}`,'PUT',{...data,idCliente:id}); else await api('/cliente','POST',data); loadClientes(); },
  },
  empleado: {
    title: d => d ? 'Editar empleado' : 'Nuevo empleado',
    section: 'empleados',
    fields: d => `
      <div class="field-row">
        <div class="field"><label class="field-label">Nombre *</label><input class="field-input" id="f_nombre" value="${esc(d?.nombre||'')}" placeholder="Ana"></div>
        <div class="field"><label class="field-label">Apellido *</label><input class="field-input" id="f_apellido" value="${esc(d?.apellido||'')}" placeholder="García"></div>
      </div>
      <div class="field-row">
        <div class="field"><label class="field-label">Puesto</label><input class="field-input" id="f_puesto" value="${esc(d?.puesto||'')}" placeholder="Recepcionista"></div>
        <div class="field"><label class="field-label">Teléfono</label><input class="field-input" id="f_telefono" value="${esc(d?.telefono||'')}" placeholder="5555-1234"></div>
      </div>`,
    validate: () => { const n=v('nombre'),a=v('apellido'); if(!n||!a){toast('Nombre y apellido obligatorios','err');return null;} return{nombre:n,apellido:a,puesto:v('puesto'),telefono:v('telefono')}; },
    save: async (data,id) => { if(id) await api(`/empleado/${id}`,'PUT',{...data,idEmpleado:id}); else await api('/empleado','POST',data); loadEmpleados(); },
  },
  reservacion: {
    title: d => d ? 'Editar reservación' : 'Nueva reservación',
    section: 'reservaciones',
    fields: async d => {
      if (!cache.clientes.length)  cache.clientes  = await api('/cliente');
      if (!cache.empleados.length) cache.empleados = await api('/empleado');
      if (!cache.hoteles.length)   cache.hoteles   = await api('/hotel');
      const cOpts = cache.clientes.map(c=>`<option value="${c.idCliente}" ${d?.idCliente===c.idCliente?'selected':''}>${c.nombre} ${c.apellido}</option>`).join('');
      const eOpts = cache.empleados.map(e=>`<option value="${e.idEmpleado}" ${d?.idEmpleado===e.idEmpleado?'selected':''}>${e.nombre} ${e.apellido}</option>`).join('');
      const hOpts = cache.hoteles.map(h=>`<option value="${h.idHotel}" ${d?.idHotel===h.idHotel?'selected':''}>${h.nombre}</option>`).join('');
      return `
        <div class="field"><label class="field-label">Cliente *</label><select class="field-select" id="f_idCliente">${cOpts}</select></div>
        <div class="field"><label class="field-label">Empleado *</label><select class="field-select" id="f_idEmpleado">${eOpts}</select></div>
        <div class="field"><label class="field-label">Hotel *</label><select class="field-select" id="f_idHotel">${hOpts}</select></div>
        <div class="field-row">
          <div class="field"><label class="field-label">Fecha Inicio *</label><input class="field-input" type="date" id="f_fechaInicio" value="${d?.fechaInicio||''}"></div>
          <div class="field"><label class="field-label">Fecha Fin *</label><input class="field-input" type="date" id="f_fechaFin" value="${d?.fechaFin||''}"></div>
        </div>
        <div class="field"><label class="field-label">Total (Q)</label><input class="field-input" type="number" id="f_total" value="${d?.total||''}" placeholder="0.00" step="0.01" min="0"></div>`;
    },
    validate: () => { const fi=v('fechaInicio'),ff=v('fechaFin'); if(!fi||!ff){toast('Fechas obligatorias','err');return null;} if(ff<fi){toast('Fecha fin inválida','err');return null;} return{idCliente:parseInt(v('idCliente')),idEmpleado:parseInt(v('idEmpleado')),idHotel:parseInt(v('idHotel')),fechaInicio:fi,fechaFin:ff,total:parseFloat(v('total'))||0}; },
    save: async (data,id) => { if(id) await api(`/reservacion/${id}`,'PUT',{...data,idReservacion:id}); else await api('/reservacion','POST',data); loadReservaciones(); },
  },
};

async function openModal(type, data = null) {
  const cfg = MODAL_CONFIGS[type];

  // Verificar permiso antes de abrir
  const action = data ? 'edit' : 'create';
  if (!can(action, cfg.section)) { toast('Sin permiso para esta acción', 'warn'); return; }

  const idKey = {hotel:'idHotel',habitacion:'idHabitacion',cliente:'idCliente',empleado:'idEmpleado',reservacion:'idReservacion'};
  modalState = { type, editId: data?.[idKey[type]] ?? null };

  document.getElementById('modalTitle').textContent = cfg.title(data);
  document.getElementById('modalBody').innerHTML    = '<div style="color:var(--muted);font-size:11px;padding:8px 0">Cargando...</div>';
  document.getElementById('modalBg').classList.add('open');

  const fields = await (typeof cfg.fields === 'function' ? cfg.fields(data) : cfg.fields);
  document.getElementById('modalBody').innerHTML = fields;
}

function closeModal() {
  document.getElementById('modalBg').classList.remove('open');
  modalState = { type: null, editId: null };
}

async function submitModal() {
  const cfg = MODAL_CONFIGS[modalState.type];
  if (!cfg) return;
  const data = cfg.validate();
  if (!data) return;

  const saveBtn = document.querySelector('.modal-foot .btn-accent');
  saveBtn.textContent = '...'; saveBtn.disabled = true;

  try {
    await cfg.save(data, modalState.editId);
    toast(modalState.editId ? 'Actualizado correctamente' : 'Creado correctamente');
    closeModal();
  } catch(e) {
    toast('Error: ' + e.message, 'err');
  } finally {
    saveBtn.textContent = 'Guardar'; saveBtn.disabled = false;
  }
}

// ── INIT ──────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
  document.addEventListener('keydown', e => {
    const loginVisible = document.getElementById('loginScreen').style.display !== 'none'
                      && !document.getElementById('loginScreen').classList.contains('hide');
    if (e.key === 'Enter' && loginVisible) doLogin();
    if (e.key === 'Escape') closeModal();
  });

  document.getElementById('modalBg').addEventListener('click', e => {
    if (e.target === document.getElementById('modalBg')) closeModal();
  });

  // Sesión existente
  const existingUser = getUser();
  if (existingUser) {
    document.getElementById('loginScreen').style.display = 'none';
    bootApp(existingUser);
  }
});
