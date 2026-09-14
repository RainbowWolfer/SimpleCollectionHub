(() => {
	const $ = (id) => document.getElementById(id);

	const groups = [
		{ id: "games", name: "游戏", parentId: null, kind: "games" },
		{ id: "indie", name: "独立游戏", parentId: "games", kind: "games" },
		{ id: "indie-done", name: "已通关", parentId: "indie", kind: "games" },
		{ id: "indie-now", name: "在玩", parentId: "indie", kind: "games" },
		{ id: "indie-wish", name: "想玩", parentId: "indie", kind: "games" },
		{ id: "aaa", name: "3A", parentId: "games", kind: "games" },
		{ id: "aaa-open", name: "开放世界", parentId: "aaa", kind: "games" },
		{ id: "music", name: "音乐", parentId: null, kind: "music" },
		{ id: "music-album", name: "专辑", parentId: "music", kind: "music" },
		{ id: "music-ost", name: "原声 / 精选", parentId: "music", kind: "music" },
		{ id: "video", name: "视频", parentId: null, kind: "video" },
		{ id: "anime", name: "动画", parentId: "video", kind: "video" },
		{ id: "anime-film", name: "剧场版 / 长篇", parentId: "anime", kind: "video" },
		{ id: "anime-tv", name: "电视动画", parentId: "anime", kind: "video" },
	];

	const items = [
		item("hades", "indie-done", "Hades", "D:\\Games\\Hades", true, {
			kicker: "Roguelike",
			pattern: "embers",
			h: 18,
			title: "HADES",
			ratio: "portrait",
			note: "想再打一条 32 热度。\n不要打开游戏也能想起：镜子先点 dash，然后再考虑攻强。",
			shots: [
				shot("embers", 14, "逃亡起手"),
				shot("gold", 28, "Boss: [REDACTED]"),
				shot("peak", 8, "Elysium"),
			],
			links: [
				{ title: "Steam", url: "https://store.steampowered.com/app/1145360/Hades/" },
				{ title: "官网", url: "https://www.supergiantgames.com/games/hades/" },
			],
		}),
		item("hk", "indie-done", "空洞骑士", "D:\\Games\\Hollow Knight", true, {
			kicker: "Metroidvania",
			pattern: "mist",
			h: 200,
			title: "HOLLOW\nKNIGHT",
			ratio: "portrait",
			note: "神居还没打完。地图里白宫殿和梦魇之王的位置已经标过。",
			shots: [
				shot("mist", 200, "Dirtmouth"),
				shot("mist", 190, "水晶山峰"),
				shot("gold", 42, "辐射村"),
			],
			links: [
				{ title: "Steam", url: "https://store.steampowered.com/app/367520" },
				{ title: "Wiki", url: "https://hollowknight.wiki/" },
			],
		}),
		item("stardew", "indie-now", "星露谷物语", "D:\\Games\\Stardew Valley", true, {
			kicker: "Farm",
			pattern: "sun",
			h: 92,
			title: "STARDEW",
			ratio: "portrait",
			note: "春 13 日之前把防风草种子买齐。存档在 Saves 子目录。",
			shots: [shot("sun", 88, "农场黄昏"), shot("sun", 110, "星之露水节")],
			links: [{ title: "官方 wiki", url: "https://stardewvalleywiki.com/" }],
		}),
		item("celeste", "indie-done", "Celeste", "D:\\Games\\Celeste", false, {
			kicker: "Platformer",
			pattern: "peak",
			h: 312,
			title: "CELESTE",
			ratio: "portrait",
			note: "目录搬家后还没重新指定路径。C 面先搁着。",
			shots: [shot("peak", 300, "第一章"), shot("peak", 280, "Summit")],
			links: [{ title: "itch.io", url: "https://www.exok.com/games/celeste/" }],
		}),
		item("elden", "aaa-open", "艾尔登法环", "D:\\Games\\Elden Ring", true, {
			kicker: "Souls",
			pattern: "gold",
			h: 38,
			title: "ELDEN\nRING",
			ratio: "portrait",
			note: "宁姆格福南的地图碎片、赐福点截过图。源地址留给下次买 DLC 用。",
			shots: [
				shot("gold", 36, "宁姆格福"),
				shot("gold", 28, "风暴山丘"),
				shot("mist", 24, "王城"),
			],
			links: [
				{ title: "Steam", url: "https://store.steampowered.com/app/1245620" },
				{ title: "地图", url: "https://eldenring.wiki.fextralife.com/Interactive+Map" },
			],
		}),
		item("yorushika", "music-album", "ヨルシカ / 幻燈", "D:\\Music\\Yorushika\\幻燈", true, {
			kicker: "Album",
			pattern: "vinyl",
			h: 210,
			title: "幻燈",
			ratio: "square",
			note: "整专循环。喜欢《花人局》和《春泥棒》。",
			shots: [shot("vinyl", 210, "封面扫描"), shot("mist", 200, "现场截帧")],
			links: [{ title: "官方", url: "https://yorushika.com/" }],
		}),
		item("kenshi", "music-album", "米津玄師 / BOOTLEG", "D:\\Music\\Kenshi Yonezu\\BOOTLEG", true, {
			kicker: "Album",
			pattern: "vinyl",
			h: 350,
			title: "BOOTLEG",
			ratio: "square",
			note: "无损在 FLAC 子文件夹。",
			shots: [shot("vinyl", 350, "封面")],
			links: [{ title: "YouTube", url: "https://www.youtube.com/@kms" }],
		}),
		item("hisaishi", "music-ost", "久石让精选", "D:\\Music\\Joe Hisaishi", true, {
			kicker: "Compilation",
			pattern: "sun",
			h: 195,
			title: "HISAISHI",
			ratio: "square",
			note: "按电影分了子目录：Totoro / Spirited Away / Howl。",
			shots: [shot("sun", 48, "天空之城"), shot("mist", 200, "千与千寻")],
			links: [],
		}),
		item("eva", "anime-film", "新世纪福音战士", "E:\\Videos\\EVA", true, {
			kicker: "Series",
			pattern: "stage",
			h: 0,
			title: "EVANGELION",
			ratio: "wide",
			note: "TV + EoE。旧字幕在 Subs。先不要跟别人借原盘路径。",
			shots: [shot("stage", 0, "第 19 话"), shot("stage", 12, "Air / 真心为你")],
			links: [{ title: "介绍页", url: "https://www.evangelion.co.jp/" }],
		}),
		item("spy", "anime-tv", "间谍过家家 第一季", "E:\\Videos\\Spy Family S1", true, {
			kicker: "Season",
			pattern: "stage",
			h: 8,
			title: "SPY×FAMILY",
			ratio: "wide",
			note: "看过到第 8 集。片头曲单独放了一份。",
			shots: [shot("stage", 12, "OP"), shot("sun", 20, "安妮亚")],
			links: [],
		}),
	];

	function item(id, groupId, name, path, exists, extra) {
		return { id, groupId, name, path, exists, ...extra };
	}
	function shot(pattern, h, caption) {
		return { id: "s" + Math.random().toString(36).slice(2, 8), pattern, h, caption };
	}
	function esc(s) {
		return String(s ?? "").replace(/[&<>"']/g, (c) => (
			{ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]
		));
	}
	function groupById(id) {
		return groups.find((g) => g.id === id);
	}
	function itemById(id) {
		return items.find((i) => i.id === id);
	}
	function childrenOf(parentId) {
		const key = parentId ?? null;
		return groups.filter((g) => (g.parentId ?? null) === key);
	}
	function descendantIds(id) {
		const out = [id];
		for (const child of childrenOf(id)) out.push(...descendantIds(child.id));
		return out;
	}
	function ancestors(id) {
		const chain = [];
		let g = groupById(id);
		while (g) {
			chain.unshift(g);
			g = groupById(g.parentId);
		}
		return chain;
	}
	function kindOf(g) {
		let cur = typeof g === "string" ? groupById(g) : g;
		while (cur) {
			if (cur.kind) return cur.kind;
			cur = groupById(cur.parentId);
		}
		return "games";
	}
	function subtreeCount(id) {
		const ids = new Set(descendantIds(id));
		return items.filter((it) => ids.has(it.groupId)).length;
	}
	function matchesQuery(it, q) {
		if (!q) return true;
		const blob = [it.name, it.path, it.note, ...(it.links || []).map((l) => l.title + l.url)].join("\n").toLowerCase();
		return blob.includes(q);
	}

	const state = {
		groupId: "all",
		viewMode: "cards",
		query: "",
		selectedId: null,
		editing: false,
		dialog: null,
		pendingImagesFor: null,
		ctxItemId: null,
		expanded: new Set(groups.filter((g) => childrenOf(g.id).length).map((g) => g.id)),
	};

	let toastTimer = 0;
	let uid = 100;

	function visibleGroupIds() {
		if (state.groupId === "all") return null;
		return new Set(descendantIds(state.groupId));
	}

	function filtered() {
		const q = state.query.trim().toLowerCase();
		const ids = visibleGroupIds();
		return items.filter((it) => {
			if (ids && !ids.has(it.groupId)) return false;
			return matchesQuery(it, q);
		});
	}

	function posterHTML(it, ratioClass) {
		const title = esc(it.title || it.name).replace(/\n/g, "<br>");
		return `
			<div class="poster ${ratioClass} pattern-${esc(it.pattern)}" style="--h:${Number(it.h) || 210}">
				<div>
					<div class="poster-kicker">${esc(it.kicker || groupById(it.groupId)?.name || "")}</div>
					<div class="poster-title">${title}</div>
				</div>
			</div>`;
	}

	function shotFace(s) {
		if (s.src) return `<img src="${esc(s.src)}" alt="${esc(s.caption || "")}">`;
		return `<div class="poster ratio-wide pattern-${esc(s.pattern || "mist")}" style="--h:${Number(s.h) || 200}"></div>`;
	}

	function treeNodeHTML(g, depth) {
		const kids = childrenOf(g.id);
		const open = state.expanded.has(g.id);
		const active = state.groupId === g.id ? "is-active" : "";
		const twistClass = kids.length ? (open ? "is-open" : "") : "is-leaf";
		const kidsHTML = kids.length && open ? kids.map((c) => treeNodeHTML(c, depth + 1)).join("") : "";
		return `
			<div class="tree-row ${active}" style="--d:${depth}">
				<button class="tree-twist ${twistClass}" type="button" data-action="toggle-group" data-id="${g.id}" aria-label="展开或折叠">▶</button>
				<button class="tree-label" type="button" data-action="select-group" data-id="${g.id}">
					<span class="tree-name">${esc(g.name)}</span>
					<span class="nav-count">${subtreeCount(g.id)}</span>
				</button>
			</div>
			${kidsHTML}`;
	}

	function renderSidebar() {
		const roots = childrenOf(null);
		$("groupNav").innerHTML = `
			<button class="nav-item ${state.groupId === "all" ? "is-active" : ""}" type="button" data-action="select-group" data-id="all">
				<span class="nav-ico">▣</span>
				<span>全部收藏</span>
				<span class="nav-count">${items.length}</span>
			</button>
			${roots.map((g) => treeNodeHTML(g, 0)).join("")}
		`;
	}

	function renderCrumbs() {
		const el = $("crumbs");
		if (!el) return;
		if (state.groupId === "all") {
			el.innerHTML = "";
			return;
		}
		const chain = ancestors(state.groupId);
		el.innerHTML = [
			`<button class="crumb" type="button" data-action="select-group" data-id="all">全部收藏</button>`,
			...chain.map((g, i) => {
				const last = i === chain.length - 1;
				return `<span class="crumb-sep">/</span><button class="crumb ${last ? "is-current" : ""}" type="button" data-action="select-group" data-id="${g.id}">${esc(g.name)}</button>`;
			}),
		].join("");
	}

	function renderToolbar() {
		const g = state.groupId === "all" ? null : groupById(state.groupId);
		const list = filtered();
		$("pageTitle").textContent = g?.name || "全部收藏";
		$("pageCount").textContent = `${list.length} 项`;
		renderCrumbs();
		document.querySelectorAll(".segment-btn").forEach((btn) => {
			btn.classList.toggle("is-active", btn.dataset.mode === state.viewMode);
		});
	}

	function sectionHTML(title, groupId, subset) {
		if (!subset.length) return "";
		const kind = kindOf(groupId) || "games";
		return `
			<section class="section">
				<div class="section-head">
					<button class="section-jump" type="button" data-action="select-group" data-id="${groupId}">${esc(title)}</button>
					<span class="muted">${subset.length}</span>
					<div class="section-line"></div>
				</div>
				<div class="card-grid" data-kind="${esc(kind)}">${subset.map(cardHTML).join("")}</div>
			</section>`;
	}

	function itemsInSubtree(list, groupId) {
		const ids = new Set(descendantIds(groupId));
		return list.filter((it) => ids.has(it.groupId));
	}

	function renderLibrary() {
		const root = $("library");
		const list = filtered();
		if (!list.length) {
			root.innerHTML = `
				<div class="empty">
					<h3>没有匹配的条目</h3>
					<p>换个关键词，或把一个已有目录加进当前分组。分组可以再嵌一套子分组。</p>
					<button class="btn btn-primary" type="button" data-action="open-add-item">添加条目</button>
				</div>`;
			return;
		}

		if (state.viewMode === "list") {
			root.innerHTML = `<div class="list">${list.map(listRow).join("")}</div>`;
			return;
		}

		if (state.groupId === "all") {
			root.innerHTML = childrenOf(null).map((rootGroup) => {
				const kids = childrenOf(rootGroup.id);
				if (!kids.length) {
					return sectionHTML(rootGroup.name, rootGroup.id, itemsInSubtree(list, rootGroup.id));
				}
				const inner = kids.map((child) => sectionHTML(`${rootGroup.name} / ${child.name}`, child.id, itemsInSubtree(list, child.id))).join("");
				const direct = list.filter((it) => it.groupId === rootGroup.id);
				return (direct.length ? sectionHTML(rootGroup.name, rootGroup.id, direct) : "") + inner;
			}).join("");
			return;
		}

		const current = groupById(state.groupId);
		const kids = childrenOf(state.groupId);
		if (!current || !kids.length) {
			root.innerHTML = `<div class="card-grid" data-kind="${esc(kindOf(current) || "games")}">${list.map(cardHTML).join("")}</div>`;
			return;
		}

		const direct = list.filter((it) => it.groupId === state.groupId);
		const blocks = [];
		if (direct.length) blocks.push(sectionHTML("本组", state.groupId, direct));
		for (const child of kids) {
			const subset = itemsInSubtree(list, child.id);
			if (subset.length) blocks.push(sectionHTML(child.name, child.id, subset));
			else blocks.push(`
				<section class="section">
					<div class="section-head">
						<button class="section-jump" type="button" data-action="select-group" data-id="${child.id}">${esc(child.name)}</button>
						<span class="muted">0</span>
						<div class="section-line"></div>
					</div>
					<p class="muted" style="margin:0 0 8px">这个子分组还是空的。</p>
				</section>`);
		}
		root.innerHTML = blocks.join("");
	}

	function cardHTML(it) {
		const ratio = it.ratio === "square" ? "ratio-square" : it.ratio === "wide" ? "ratio-wide" : "ratio-portrait";
		const selected = state.selectedId === it.id ? "is-selected" : "";
		return `
			<button class="card ${selected}" type="button" data-action="select-item" data-id="${it.id}">
				${posterHTML(it, ratio)}
				<div class="card-meta">
					<div class="card-name">${esc(it.name)}</div>
					<div class="card-path">${esc(it.path)}</div>
					<div class="card-flags">
						<span class="chip ${it.exists ? "ok" : "warn"}">${it.exists ? "路径可用" : "路径丢失"}</span>
						<span class="chip">${(it.shots || []).length} 图</span>
					</div>
				</div>
			</button>`;
	}

	function listRow(it) {
		const selected = state.selectedId === it.id ? "is-selected" : "";
		const pathNames = ancestors(it.groupId).map((g) => g.name).join(" / ");
		return `
			<button class="list-row ${selected}" type="button" data-action="select-item" data-id="${it.id}">
				<div class="list-thumb">${posterHTML(it, "ratio-square")}</div>
				<div>
					<div class="list-name">${esc(it.name)}</div>
					<div class="muted">${esc(pathNames)}</div>
				</div>
				<div class="list-path-col list-path">${esc(it.path)}</div>
				<div class="list-flag"><span class="chip ${it.exists ? "ok" : "warn"}">${it.exists ? "可用" : "丢失"}</span></div>
			</button>`;
	}

	function groupSelectOptions(selectedId, { includeEmpty = false, emptyLabel = "（顶级分组）" } = {}) {
		const rows = [];
		if (includeEmpty) rows.push(`<option value="" ${!selectedId ? "selected" : ""}>${esc(emptyLabel)}</option>`);
		function walk(parentId, prefix) {
			for (const g of childrenOf(parentId)) {
				const sel = g.id === selectedId ? "selected" : "";
				rows.push(`<option value="${g.id}" ${sel}>${esc(prefix + g.name)}</option>`);
				walk(g.id, prefix + "　");
			}
		}
		walk(null, "");
		return rows.join("");
	}

	function renderDetail() {
		const pane = $("detail");
		const it = itemById(state.selectedId);
		if (!it) {
			pane.hidden = true;
			pane.innerHTML = "";
			return;
		}
		pane.hidden = false;
		const g = groupById(it.groupId);
		const shots = it.shots || [];
		const links = it.links || [];
		const pathNames = ancestors(it.groupId).map((x) => x.name).join(" / ");

		pane.innerHTML = `
			<div class="detail-head">
				<h2>${esc(it.name)}</h2>
				<button class="icon-btn" type="button" data-action="toggle-edit" title="编辑">${state.editing ? "✓" : "✎"}</button>
				<button class="icon-btn" type="button" data-action="close-detail" title="关闭">✕</button>
			</div>
			<div class="detail-body">
				<div class="path-row">
					<span class="chip ${it.exists ? "ok" : "warn"}">${it.exists ? "可用" : "丢失"}</span>
					<div class="path-text" title="${esc(it.path)}">${esc(it.path)}</div>
					<button class="btn" type="button" data-action="open-path" data-id="${it.id}">打开目录</button>
				</div>
				<div class="field">
					<div class="field-label">分组</div>
					${state.editing
						? `<select data-role="item-group">${groupSelectOptions(it.groupId)}</select>`
						: `<div class="note">${esc(pathNames || g?.name || "")}</div>`}
				</div>
				<div class="field">
					<div class="field-label">封面 · ${esc(g?.name || "")}</div>
					${posterHTML(it, it.ratio === "square" ? "ratio-square" : it.ratio === "wide" ? "ratio-wide" : "ratio-portrait")}
				</div>
				<div class="field">
					<div class="field-label">
						<span>截图</span>
						${state.editing ? `<button class="btn btn-ghost" type="button" data-action="pick-images" data-id="${it.id}">添加图片</button>` : ""}
					</div>
					<div class="shots">
						${shots.map((s) => `
							<button class="shot" type="button" data-action="open-shot" data-item="${it.id}" data-shot="${s.id}">
								${shotFace(s)}
								${s.caption ? `<span class="shot-cap">${esc(s.caption)}</span>` : ""}
							</button>`).join("")}
						${state.editing ? `<button class="shot-add" type="button" data-action="pick-images" data-id="${it.id}">＋ 图片</button>` : ""}
					</div>
				</div>
				<div class="field">
					<div class="field-label">备注</div>
					${state.editing
						? `<textarea class="note-input" data-role="note">${esc(it.note || "")}</textarea>`
						: `<div class="note">${esc(it.note || "还没有备注。")}</div>`}
				</div>
				<div class="field">
					<div class="field-label">相关链接</div>
					<div class="links">
						${links.map((l, idx) => `
							<a class="link-row" href="${esc(l.url)}" target="_blank" rel="noopener">
								<div style="flex:1;min-width:0">
									<div class="link-title">${esc(l.title)}</div>
									<div class="link-url">${esc(l.url)}</div>
								</div>
								${state.editing ? `<button class="icon-btn" type="button" data-action="remove-link" data-id="${it.id}" data-idx="${idx}">✕</button>` : ""}
							</a>`).join("") || (!state.editing ? `<div class="muted">没有链接</div>` : "")}
						${state.editing ? `
							<div class="link-add">
								<input data-role="link-title" placeholder="标题，如 Steam">
								<input data-role="link-url" placeholder="https://">
								<button class="btn" type="button" data-action="add-link" data-id="${it.id}">添加</button>
							</div>` : ""}
					</div>
				</div>
			</div>
			<div class="detail-actions">
				<button class="btn btn-primary" type="button" data-action="open-path" data-id="${it.id}">打开原目录</button>
				<button class="btn" type="button" data-action="toggle-edit">${state.editing ? "完成" : "编辑字段"}</button>
				<button class="btn btn-danger" type="button" data-action="remove-item" data-id="${it.id}">从库中移除</button>
			</div>`;
	}

	function renderAll() {
		renderSidebar();
		renderToolbar();
		renderLibrary();
		renderDetail();
	}

	function toast(msg) {
		const el = $("toast");
		el.textContent = msg;
		el.hidden = false;
		clearTimeout(toastTimer);
		toastTimer = setTimeout(() => { el.hidden = true; }, 2400);
	}

	function closeDialog() {
		state.dialog = null;
		$("dialog").hidden = true;
		$("overlay").hidden = true;
	}

	function openDialog(html) {
		$("dialog").innerHTML = html;
		$("dialog").hidden = false;
		$("overlay").hidden = false;
		const first = $("dialog").querySelector("input, select, textarea, button");
		first?.focus();
	}

	function defaultItemGroup() {
		if (state.groupId !== "all" && groupById(state.groupId)) return state.groupId;
		return "indie-done";
	}

	function openAddItem(presetGroupId) {
		state.dialog = "add-item";
		const selected = presetGroupId || defaultItemGroup();
		openDialog(`
			<h3>添加条目</h3>
			<p class="hint">绑定已经在磁盘上的目录或文件。原文件不会被移动或复制。分组可以是树里的任意一层。</p>
			<div class="form-row">
				<label>已有路径</label>
				<div class="path-pick">
					<input id="newPath" placeholder="例如 D:\\Games\\SomeGame">
					<button class="btn" type="button" data-action="browse-folder">浏览…</button>
				</div>
				<div class="folder-list">
					<button class="folder-opt" type="button" data-action="use-folder" data-path="D:\\Games\\Hades" data-name="Hades" data-group="indie-done"><b>Hades</b><small>D:\\Games\\Hades</small></button>
					<button class="folder-opt" type="button" data-action="use-folder" data-path="D:\\Music\\New Album" data-name="New Album" data-group="music-album"><b>New Album</b><small>D:\\Music\\New Album</small></button>
					<button class="folder-opt" type="button" data-action="use-folder" data-path="E:\\Videos\\Film" data-name="Film" data-group="anime-film"><b>Film</b><small>E:\\Videos\\Film</small></button>
				</div>
			</div>
			<div class="form-row">
				<label>显示名称</label>
				<input id="newName" placeholder="默认使用文件夹名">
			</div>
			<div class="form-row">
				<label>放到分组</label>
				<select id="newGroup">${groupSelectOptions(selected)}</select>
			</div>
			<div class="form-row">
				<label>备注（可选）</label>
				<textarea id="newNote" placeholder="通关进度、片源、想记的一句话…"></textarea>
			</div>
			<div class="dialog-actions">
				<button class="btn" type="button" data-action="close-dialog">取消</button>
				<button class="btn btn-primary" type="button" data-action="confirm-add-item">添加到库</button>
			</div>
		`);
	}

	function openAddGroup(presetParentId) {
		state.dialog = "add-group";
		const parent = presetParentId === undefined
			? (state.groupId === "all" ? "" : state.groupId)
			: (presetParentId || "");
		openDialog(`
			<h3>新建分组</h3>
			<p class="hint">分组可以嵌套。例如「游戏 → 独立游戏 → 想玩」。条目仍然指向原来的目录。</p>
			<div class="form-row">
				<label>分组名称</label>
				<input id="newGroupName" placeholder="例如 想玩、同人音乐、剧场版">
			</div>
			<div class="form-row">
				<label>放在</label>
				<select id="newGroupParent">${groupSelectOptions(parent, { includeEmpty: true, emptyLabel: "（顶级，和「游戏」同级）" })}</select>
			</div>
			<div class="dialog-actions">
				<button class="btn" type="button" data-action="close-dialog">取消</button>
				<button class="btn btn-primary" type="button" data-action="confirm-add-group">创建</button>
			</div>
		`);
	}

	function openHelp() {
		state.dialog = "help";
		openDialog(`
			<h3>原型 2：嵌套分组</h3>
			<p class="hint">在原型 1 的卡片墙之上，左侧改成树。旧版仍在 ui-prototype-1。</p>
			<ul class="help-list">
				<li>点名称选中分组；点箭头展开 / 折叠。</li>
				<li>选中父分组时，中间按子分组分块显示，包含更下层的条目。</li>
				<li>右键分组：新建子分组、把条目加到这里、删除空分组。</li>
				<li>面包屑可以回到任意一层。</li>
			</ul>
			<div class="dialog-actions">
				<button class="btn btn-primary" type="button" data-action="close-dialog">知道了</button>
			</div>
		`);
	}

	function confirmAddItem() {
		const path = $("newPath").value.trim();
		const groupId = $("newGroup").value;
		let name = $("newName").value.trim();
		const note = $("newNote").value.trim();
		if (!path) { toast("先指定一个已有路径"); return; }
		if (!groupById(groupId)) { toast("先选一个分组"); return; }
		if (!name) {
			const parts = path.split(/[/\\]/).filter(Boolean);
			name = parts[parts.length - 1] || path;
		}
		const kind = kindOf(groupId);
		const hue = Math.floor(Math.random() * 360);
		const ratio = kind === "music" ? "square" : kind === "video" ? "wide" : "portrait";
		const pattern = kind === "music" ? "vinyl" : kind === "video" ? "stage" : "embers";
		const created = {
			id: "n" + (++uid),
			groupId,
			name,
			path,
			exists: true,
			kicker: groupById(groupId)?.name || "",
			pattern,
			h: hue,
			title: name.toUpperCase(),
			ratio,
			note,
			shots: [],
			links: [],
		};
		items.unshift(created);
		selectGroup(groupId);
		state.selectedId = created.id;
		state.editing = true;
		closeDialog();
		renderAll();
		toast(`已绑定 ${path}，原文件还在原地`);
	}

	function confirmAddGroup() {
		const name = $("newGroupName").value.trim();
		if (!name) { toast("给分组起个名字"); return; }
		const parentId = $("newGroupParent").value || null;
		const id = "g" + (++uid);
		const parent = parentId ? groupById(parentId) : null;
		groups.push({
			id,
			name,
			parentId,
			kind: parent ? kindOf(parent) : "games",
		});
		if (parentId) state.expanded.add(parentId);
		state.expanded.add(id);
		selectGroup(id);
		closeDialog();
		renderAll();
		toast(`已创建分组「${name}」`);
	}

	function selectGroup(id) {
		state.groupId = id;
		state.selectedId = null;
		state.editing = false;
		if (id !== "all" && childrenOf(id).length) state.expanded.add(id);
		ancestors(id).forEach((g) => state.expanded.add(g.id));
	}

	function toggleGroup(id) {
		if (state.expanded.has(id)) state.expanded.delete(id);
		else state.expanded.add(id);
		renderSidebar();
	}

	function deleteGroup(id) {
		const g = groupById(id);
		if (!g) return;
		if (childrenOf(id).length) {
			toast("先删掉或移走子分组");
			return;
		}
		const owned = items.filter((it) => it.groupId === id);
		if (owned.length) {
			if (!confirm(`「${g.name}」里还有 ${owned.length} 个条目。\n删除分组后，它们会移到上一层。继续？`)) return;
			const parentId = g.parentId;
			if (!parentId) {
				toast("顶级分组里还有条目，先移走再删");
				return;
			}
			owned.forEach((it) => { it.groupId = parentId; });
		} else if (!confirm(`删除分组「${g.name}」？`)) {
			return;
		}
		const idx = groups.findIndex((x) => x.id === id);
		if (idx >= 0) groups.splice(idx, 1);
		if (state.groupId === id) state.groupId = g.parentId || "all";
		renderAll();
		toast("分组已删除");
	}

	function selectItem(id) {
		state.selectedId = id;
		state.editing = false;
		hideCtx();
		renderAll();
	}

	function removeItem(id) {
		const it = itemById(id);
		if (!it) return;
		if (!confirm(`从库中移除「${it.name}」？\n原目录不会被删除。`)) return;
		const idx = items.findIndex((x) => x.id === id);
		if (idx >= 0) items.splice(idx, 1);
		if (state.selectedId === id) state.selectedId = null;
		state.editing = false;
		renderAll();
		toast("已从库中移除，磁盘文件未改动");
	}

	function openPath(id) {
		const it = itemById(id);
		if (!it) return;
		if (!it.exists) toast("路径已经找不到，先重新指定目录");
		else toast(`原型：将在资源管理器打开\n${it.path}`);
	}

	function openShot(itemId, shotId) {
		const it = itemById(itemId);
		const s = it?.shots?.find((x) => x.id === shotId);
		if (!s) return;
		$("lightboxStage").innerHTML = shotFace(s);
		$("lightboxCap").textContent = [it.name, s.caption].filter(Boolean).join(" · ");
		$("lightbox").hidden = false;
	}

	function hideCtx() { $("contextMenu").hidden = true; }

	function placeCtx(x, y, html) {
		const menu = $("contextMenu");
		menu.innerHTML = html;
		menu.hidden = false;
		const pad = 8;
		const app = document.querySelector(".app").getBoundingClientRect();
		const r = menu.getBoundingClientRect();
		menu.style.left = Math.min(Math.max(pad, x - app.left), app.width - r.width - pad) + "px";
		menu.style.top = Math.min(Math.max(pad, y - app.top), app.height - r.height - pad) + "px";
	}

	function showItemCtx(x, y, itemId) {
		state.ctxItemId = itemId;
		placeCtx(x, y, `
			<button type="button" data-action="open-path" data-id="${itemId}">打开原目录</button>
			<button type="button" data-action="select-item" data-id="${itemId}">查看详情</button>
			<button type="button" data-action="edit-item" data-id="${itemId}">编辑字段</button>
			<div class="sep"></div>
			<button type="button" data-action="remove-item" data-id="${itemId}">从库中移除</button>`);
	}

	function showGroupCtx(x, y, groupId) {
		placeCtx(x, y, `
			<button type="button" data-action="select-group" data-id="${groupId}">查看此分组</button>
			<button type="button" data-action="open-add-group" data-parent="${groupId}">新建子分组</button>
			<button type="button" data-action="open-add-item" data-group="${groupId}">添加条目到此分组</button>
			<div class="sep"></div>
			<button type="button" data-action="delete-group" data-id="${groupId}">删除分组</button>`);
	}

	function persistEdits() {
		if (!state.editing || !state.selectedId) return;
		const it = itemById(state.selectedId);
		if (!it) return;
		const ta = document.querySelector("[data-role='note']");
		if (ta) it.note = ta.value;
		const sel = document.querySelector("[data-role='item-group']");
		if (sel && groupById(sel.value)) it.groupId = sel.value;
	}

	document.addEventListener("click", (e) => {
		const hit = e.target.closest("[data-action]");
		if (!hit) {
			if (!e.target.closest(".ctx")) hideCtx();
			return;
		}
		const action = hit.dataset.action;
		if (action === "remove-link") e.preventDefault();
		if (action !== "toggle-edit") persistEdits();

		switch (action) {
			case "noop":
				toast("原型里的窗口按钮没有接系统行为");
				break;
			case "select-group":
				hideCtx();
				selectGroup(hit.dataset.id);
				renderAll();
				break;
			case "toggle-group":
				toggleGroup(hit.dataset.id);
				break;
			case "view-mode":
				state.viewMode = hit.dataset.mode;
				renderAll();
				break;
			case "select-item":
				selectItem(hit.dataset.id);
				break;
			case "edit-item":
				state.selectedId = hit.dataset.id;
				state.editing = true;
				hideCtx();
				renderAll();
				break;
			case "close-detail":
				state.selectedId = null;
				state.editing = false;
				renderDetail();
				renderLibrary();
				break;
			case "toggle-edit":
				persistEdits();
				state.editing = !state.editing;
				if (!state.editing) renderAll();
				else renderDetail();
				break;
			case "open-add-item":
				hideCtx();
				openAddItem(hit.dataset.group);
				break;
			case "open-add-group":
				hideCtx();
				openAddGroup(hit.dataset.parent);
				break;
			case "delete-group":
				hideCtx();
				deleteGroup(hit.dataset.id);
				break;
			case "open-help":
				openHelp();
				break;
			case "close-dialog":
				closeDialog();
				break;
			case "confirm-add-item":
				confirmAddItem();
				break;
			case "confirm-add-group":
				confirmAddGroup();
				break;
			case "use-folder":
				$("newPath").value = hit.dataset.path;
				if (!$("newName").value) $("newName").value = hit.dataset.name || "";
				if (hit.dataset.group) $("newGroup").value = hit.dataset.group;
				$("dialog").querySelectorAll(".folder-opt").forEach((el) => el.classList.toggle("is-on", el === hit));
				break;
			case "browse-folder":
				$("folderPicker").click();
				break;
			case "open-path":
				openPath(hit.dataset.id);
				break;
			case "remove-item":
				hideCtx();
				removeItem(hit.dataset.id);
				break;
			case "open-shot":
				openShot(hit.dataset.item, hit.dataset.shot);
				break;
			case "close-lightbox":
				$("lightbox").hidden = true;
				break;
			case "pick-images":
				state.pendingImagesFor = hit.dataset.id;
				$("imagePicker").click();
				break;
			case "add-link": {
				const it = itemById(hit.dataset.id);
				const title = document.querySelector("[data-role='link-title']")?.value.trim();
				const url = document.querySelector("[data-role='link-url']")?.value.trim();
				if (!it || !url) { toast("链接地址不能空"); break; }
				it.links = it.links || [];
				it.links.push({ title: title || url, url });
				renderDetail();
				break;
			}
			case "remove-link": {
				const it = itemById(hit.dataset.id);
				if (!it) break;
				it.links.splice(Number(hit.dataset.idx), 1);
				renderDetail();
				break;
			}
		}
	});

	document.addEventListener("contextmenu", (e) => {
		const card = e.target.closest("[data-action='select-item']");
		if (card) {
			e.preventDefault();
			showItemCtx(e.clientX, e.clientY, card.dataset.id);
			return;
		}
		const groupHit = e.target.closest("[data-action='select-group']");
		if (groupHit && groupHit.dataset.id && groupHit.dataset.id !== "all") {
			e.preventDefault();
			showGroupCtx(e.clientX, e.clientY, groupHit.dataset.id);
		}
	});

	document.addEventListener("keydown", (e) => {
		if (e.key !== "Escape") return;
		if (!$("lightbox").hidden) { $("lightbox").hidden = true; return; }
		if (!$("dialog").hidden) { closeDialog(); return; }
		if (state.selectedId) { state.selectedId = null; state.editing = false; renderAll(); }
		hideCtx();
	});

	$("searchInput").addEventListener("input", (e) => {
		state.query = e.target.value;
		renderToolbar();
		renderLibrary();
	});

	$("overlay").addEventListener("click", closeDialog);
	$("lightbox").addEventListener("click", (e) => {
		if (e.target.id === "lightbox" || e.target.dataset.action === "close-lightbox") $("lightbox").hidden = true;
	});

	$("folderPicker").addEventListener("change", (e) => {
		const files = [...(e.target.files || [])];
		e.target.value = "";
		if (!files.length) return;
		const rel = files[0].webkitRelativePath || files[0].name;
		const folder = rel.split(/[/\\]/)[0] || "SelectedFolder";
		const path = `D:\\Collections\\${folder}`;
		if ($("newPath")) {
			$("newPath").value = path;
			if ($("newName") && !$("newName").value) $("newName").value = folder;
			toast("浏览器拿不到真实盘符，原型里写成 " + path);
		}
	});

	$("imagePicker").addEventListener("change", (e) => {
		const files = [...(e.target.files || [])];
		const it = itemById(state.pendingImagesFor);
		e.target.value = "";
		if (!it || !files.length) return;
		it.shots = it.shots || [];
		files.forEach((file) => {
			const reader = new FileReader();
			reader.onload = () => {
				it.shots.push({ id: "s" + (++uid), src: reader.result, caption: file.name });
				if (state.selectedId === it.id) renderDetail();
				renderLibrary();
			};
			reader.readAsDataURL(file);
		});
	});

	renderAll();
})();
