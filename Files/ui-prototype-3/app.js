(() => {
	const $ = (id) => document.getElementById(id);
	let uid = 100;
	const fid = () => "f" + (++uid);
	const iid = () => "s" + (++uid);

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

	function pic(pattern, h, caption) {
		return { id: iid(), pattern, h, caption };
	}
	function fImages(label, values) {
		return { id: fid(), type: "images", label, values };
	}
	function fText(label, value) {
		return { id: fid(), type: "text", label, value };
	}
	function fLinks(label, values) {
		return { id: fid(), type: "links", label, values };
	}
	function defaultFields() {
		return [fImages("封面", []), fText("备注", ""), fLinks("相关链接", [])];
	}

	const items = [
		item("hades", "indie-done", "Hades", "D:\\Games\\Hades", true, "2026-01-08", ["Roguelike", "Steam"], {
			kicker: "Roguelike", pattern: "embers", h: 18, title: "HADES", ratio: "portrait",
			fields: [
				fImages("封面", [pic("embers", 18, "盒面")]),
				fImages("截图", [pic("embers", 14, "逃亡起手"), pic("gold", 28, "Boss: [REDACTED]"), pic("peak", 8, "Elysium")]),
				fText("备注", "想再打一条 32 热度。\n不要打开游戏也能想起：镜子先点 dash，然后再考虑攻强。"),
				fLinks("相关链接", [
					{ title: "Steam", url: "https://store.steampowered.com/app/1145360/Hades/" },
					{ title: "官网", url: "https://www.supergiantgames.com/games/hades/" },
				]),
			],
		}),
		item("hk", "indie-done", "空洞骑士", "D:\\Games\\Hollow Knight", true, "2026-01-20", ["Metroidvania", "Steam"], {
			kicker: "Metroidvania", pattern: "mist", h: 200, title: "HOLLOW\nKNIGHT", ratio: "portrait",
			fields: [
				fImages("封面", [pic("mist", 200, "盒面")]),
				fImages("截图", [pic("mist", 200, "Dirtmouth"), pic("mist", 190, "水晶山峰"), pic("gold", 42, "辐射村")]),
				fText("备注", "神居还没打完。地图里白宫殿和梦魇之王的位置已经标过。"),
				fLinks("相关链接", [
					{ title: "Steam", url: "https://store.steampowered.com/app/367520" },
					{ title: "Wiki", url: "https://hollowknight.wiki/" },
				]),
			],
		}),
		item("stardew", "indie-now", "星露谷物语", "D:\\Games\\Stardew Valley", true, "2026-02-02", ["农场", "Steam"], {
			kicker: "Farm", pattern: "sun", h: 92, title: "STARDEW", ratio: "portrait",
			fields: [
				fImages("封面", [pic("sun", 92, "盒面")]),
				fImages("截图", [pic("sun", 88, "农场黄昏"), pic("sun", 110, "星之露水节")]),
				fText("备注", "春 13 日之前把防风草种子买齐。存档在 Saves 子目录。"),
				fLinks("相关链接", [{ title: "官方 wiki", url: "https://stardewvalleywiki.com/" }]),
			],
		}),
		item("celeste", "indie-done", "Celeste", "D:\\Games\\Celeste", false, "2026-02-14", ["平台"], {
			kicker: "Platformer", pattern: "peak", h: 312, title: "CELESTE", ratio: "portrait",
			fields: [
				fImages("封面", [pic("peak", 312, "盒面")]),
				fImages("截图", [pic("peak", 300, "第一章"), pic("peak", 280, "Summit")]),
				fText("备注", "目录搬家后还没重新指定路径。C 面先搁着。"),
				fLinks("相关链接", [{ title: "itch.io", url: "https://www.exok.com/games/celeste/" }]),
			],
		}),
		item("elden", "aaa-open", "艾尔登法环", "D:\\Games\\Elden Ring", true, "2026-03-01", ["Souls", "Steam"], {
			kicker: "Souls", pattern: "gold", h: 38, title: "ELDEN\nRING", ratio: "portrait",
			fields: [
				fImages("封面", [pic("gold", 38, "盒面")]),
				fImages("截图", [pic("gold", 36, "宁姆格福"), pic("gold", 28, "风暴山丘"), pic("mist", 24, "王城")]),
				fText("备注", "宁姆格福南的地图碎片、赐福点截过图。源地址留给下次买 DLC 用。"),
				fLinks("相关链接", [
					{ title: "Steam", url: "https://store.steampowered.com/app/1245620" },
					{ title: "地图", url: "https://eldenring.wiki.fextralife.com/Interactive+Map" },
				]),
			],
		}),
		item("yorushika", "music-album", "ヨルシカ / 幻燈", "D:\\Music\\Yorushika\\幻燈", true, "2026-03-18", ["J-Pop"], {
			kicker: "Album", pattern: "vinyl", h: 210, title: "幻燈", ratio: "square",
			fields: [
				fImages("封面", [pic("vinyl", 210, "封面扫描")]),
				fText("备注", "整专循环。喜欢《花人局》和《春泥棒》。"),
				fLinks("相关链接", [{ title: "官方", url: "https://yorushika.com/" }]),
			],
		}),
		item("kenshi", "music-album", "米津玄師 / BOOTLEG", "D:\\Music\\Kenshi Yonezu\\BOOTLEG", true, "2026-04-02", ["J-Pop"], {
			kicker: "Album", pattern: "vinyl", h: 350, title: "BOOTLEG", ratio: "square",
			fields: [
				fImages("封面", [pic("vinyl", 350, "封面")]),
				fLinks("相关链接", [{ title: "YouTube", url: "https://www.youtube.com/@kms" }]),
			],
		}),
		item("hisaishi", "music-ost", "久石让精选", "D:\\Music\\Joe Hisaishi", true, "2026-04-20", ["OST"], {
			kicker: "Compilation", pattern: "sun", h: 195, title: "HISAISHI", ratio: "square",
			fields: [
				fImages("封面", [pic("sun", 48, "天空之城")]),
				fText("备注", "按电影分了子目录：Totoro / Spirited Away / Howl。"),
			],
		}),
		item("eva", "anime-film", "新世纪福音战士", "E:\\Videos\\EVA", true, "2026-05-06", ["动画"], {
			kicker: "Series", pattern: "stage", h: 0, title: "EVANGELION", ratio: "wide",
			fields: [
				fImages("封面", [pic("stage", 0, "海报")]),
				fImages("截图", [pic("stage", 0, "第 19 话"), pic("stage", 12, "Air / 真心为你")]),
				fText("备注", "TV + EoE。旧字幕在 Subs。先不要跟别人借原盘路径。"),
				fLinks("相关链接", [{ title: "介绍页", url: "https://www.evangelion.co.jp/" }]),
			],
		}),
		item("spy", "anime-tv", "间谍过家家 第一季", "E:\\Videos\\Spy Family S1", true, "2026-05-22", ["动画", "追番"], {
			kicker: "Season", pattern: "stage", h: 8, title: "SPY×FAMILY", ratio: "wide",
			fields: [
				fImages("封面", [pic("stage", 12, "海报")]),
				fText("备注", "看过到第 8 集。片头曲单独放了一份。"),
			],
		}),
		item("loose", "indie-wish", "还没整理的游戏", "D:\\Games\\Backlog\\Something", true, "2026-06-01", [], {
			kicker: "Wishlist", pattern: "mist", h: 220, title: "TBD", ratio: "portrait",
			fields: [],
		}),
	];

	function item(id, groupId, name, path, exists, addedAt, tags, extra) {
		return { id, groupId, name, path, exists, addedAt, tags: tags || [], ...extra };
	}

	function esc(s) {
		return String(s ?? "").replace(/[&<>"']/g, (c) => (
			{ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]
		));
	}
	function groupById(id) { return groups.find((g) => g.id === id); }
	function itemById(id) { return items.find((i) => i.id === id); }
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

	function fieldsOf(it, type) {
		return (it.fields || []).filter((f) => f.type === type);
	}
	function imageCount(it) {
		return fieldsOf(it, "images").reduce((n, f) => n + (f.values || []).length, 0);
	}
	function hasText(it) {
		return fieldsOf(it, "text").some((f) => (f.value || "").trim());
	}
	function hasLinks(it) {
		return fieldsOf(it, "links").some((f) => (f.values || []).length);
	}
	function firstImage(it) {
		for (const f of fieldsOf(it, "images")) {
			if (f.values && f.values[0]) return f.values[0];
		}
		return null;
	}
	function fieldBlob(it) {
		return (it.fields || []).map((f) => {
			if (f.type === "text") return f.label + " " + (f.value || "");
			if (f.type === "links") return (f.values || []).map((l) => l.title + " " + l.url).join(" ");
			return f.label + " " + (f.values || []).map((v) => v.caption || "").join(" ");
		}).join("\n");
	}
	function matchesQuery(it, q) {
		if (!q) return true;
		const blob = [it.name, it.path, (it.tags || []).join(" "), fieldBlob(it)].join("\n").toLowerCase();
		return blob.includes(q);
	}
	function matchesIncomplete(it) {
		const mode = state.incomplete;
		if (mode === "all") return true;
		if (mode === "path") return !it.exists;
		if (mode === "images") return imageCount(it) === 0;
		if (mode === "text") return !hasText(it);
		if (mode === "links") return !hasLinks(it);
		return !it.exists || imageCount(it) === 0 || !hasText(it) || !hasLinks(it);
	}
	function matchesTags(it) {
		if (!state.tagFilters.size) return true;
		const tags = it.tags || [];
		for (const t of state.tagFilters) if (tags.includes(t)) return true;
		return false;
	}
	function sortItems(list) {
		const arr = list.slice();
		const key = state.sortBy;
		arr.sort((a, b) => {
			if (key === "name-desc") return b.name.localeCompare(a.name, "zh");
			if (key === "added-desc") return String(b.addedAt).localeCompare(String(a.addedAt)) || b.name.localeCompare(a.name, "zh");
			if (key === "added-asc") return String(a.addedAt).localeCompare(String(b.addedAt)) || a.name.localeCompare(b.name, "zh");
			return a.name.localeCompare(b.name, "zh");
		});
		return arr;
	}
	function allTags() {
		const set = new Set();
		items.forEach((it) => (it.tags || []).forEach((t) => set.add(t)));
		return [...set].sort((a, b) => a.localeCompare(b, "zh"));
	}

	const state = {
		groupId: "all",
		viewMode: "cards",
		query: "",
		selectedId: null,
		editing: false,
		dialog: null,
		pendingImages: null,
		incomplete: "all",
		sortBy: "name-asc",
		tagFilters: new Set(),
		expanded: new Set(groups.filter((g) => childrenOf(g.id).length).map((g) => g.id)),
	};

	function visibleGroupIds() {
		if (state.groupId === "all") return null;
		return new Set(descendantIds(state.groupId));
	}
	function filtered() {
		const q = state.query.trim().toLowerCase();
		const ids = visibleGroupIds();
		const list = items.filter((it) => {
			if (ids && !ids.has(it.groupId)) return false;
			if (!matchesQuery(it, q)) return false;
			if (!matchesIncomplete(it)) return false;
			if (!matchesTags(it)) return false;
			return true;
		});
		return sortItems(list);
	}
	function filtersActive() {
		return state.query.trim() || state.incomplete !== "all" || state.tagFilters.size;
	}

	function posterHTML(it, ratioClass) {
		const cover = firstImage(it);
		if (cover?.src) {
			return `<div class="poster ${ratioClass}"><img src="${esc(cover.src)}" alt=""></div>`;
		}
		const title = esc(it.title || it.name).replace(/\n/g, "<br>");
		return `
			<div class="poster ${ratioClass} pattern-${esc(it.pattern || "mist")}" style="--h:${Number(it.h) || 210}">
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
		renderTags();
	}

	function renderTags() {
		const tags = allTags();
		const bar = $("tagBar");
		if (!tags.length) {
			bar.innerHTML = `<span class="tag-empty">还没有标签</span>`;
			return;
		}
		bar.innerHTML = tags.map((t) => `
			<button class="tag-chip ${state.tagFilters.has(t) ? "is-on" : ""}" type="button" data-action="toggle-tag" data-tag="${esc(t)}">${esc(t)}</button>
		`).join("");
	}

	function renderCrumbs() {
		const el = $("crumbs");
		if (state.groupId === "all") { el.innerHTML = ""; return; }
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
		const totalInScope = items.filter((it) => {
			const ids = visibleGroupIds();
			return !ids || ids.has(it.groupId);
		}).length;
		$("pageCount").textContent = filtersActive() ? `${list.length} / ${totalInScope} 项` : `${list.length} 项`;
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
					<p>换个关键词、标签或完整度筛选。分组树本身不会因为筛选而改变。</p>
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
				if (!kids.length) return sectionHTML(rootGroup.name, rootGroup.id, itemsInSubtree(list, rootGroup.id));
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
			if (subset.length) {
				blocks.push(sectionHTML(child.name, child.id, subset));
			} else if (!filtersActive()) {
				blocks.push(`
					<section class="section">
						<div class="section-head">
							<button class="section-jump" type="button" data-action="select-group" data-id="${child.id}">${esc(child.name)}</button>
							<span class="muted">0</span>
							<div class="section-line"></div>
						</div>
						<p class="muted" style="margin:0 0 8px">这个子分组还是空的。</p>
					</section>`);
			}
		}
		root.innerHTML = blocks.join("") || `<div class="empty"><h3>没有匹配的条目</h3></div>`;
	}

	function cardHTML(it) {
		const ratio = it.ratio === "square" ? "ratio-square" : it.ratio === "wide" ? "ratio-wide" : "ratio-portrait";
		const selected = state.selectedId === it.id ? "is-selected" : "";
		const tags = (it.tags || []).slice(0, 2).map((t) => `<span class="chip tag">${esc(t)}</span>`).join("");
		return `
			<button class="card ${selected}" type="button" data-action="select-item" data-id="${it.id}">
				${posterHTML(it, ratio)}
				<div class="card-meta">
					<div class="card-name">${esc(it.name)}</div>
					<div class="card-path">${esc(it.path)}</div>
					<div class="card-flags">
						<span class="chip ${it.exists ? "ok" : "warn"}">${it.exists ? "路径可用" : "路径丢失"}</span>
						<span class="chip">${imageCount(it)} 图</span>
						${tags}
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

	function typeLabel(type) {
		return { images: "图片", text: "文字", links: "链接" }[type] || type;
	}

	function fieldBodyHTML(it, field) {
		if (field.type === "images") {
			const values = field.values || [];
			return `<div class="shots">
				${values.map((s) => `
					<button class="shot" type="button" data-action="open-shot" data-item="${it.id}" data-field="${field.id}" data-shot="${s.id}">
						${shotFace(s)}
						${s.caption ? `<span class="shot-cap">${esc(s.caption)}</span>` : ""}
					</button>`).join("")}
				${state.editing ? `<button class="shot-add" type="button" data-action="pick-images" data-id="${it.id}" data-field="${field.id}">＋ 图片</button>` : ""}
			</div>`;
		}
		if (field.type === "text") {
			if (state.editing) {
				return `<textarea class="note-input" data-role="field-text" data-field="${field.id}">${esc(field.value || "")}</textarea>`;
			}
			return `<div class="note">${esc(field.value || "还没有文字。")}</div>`;
		}
		const links = field.values || [];
		return `<div class="links">
			${links.map((l, idx) => `
				<a class="link-row" href="${esc(l.url)}" target="_blank" rel="noopener">
					<div style="flex:1;min-width:0">
						<div class="link-title">${esc(l.title)}</div>
						<div class="link-url">${esc(l.url)}</div>
					</div>
					${state.editing ? `<button class="icon-btn" type="button" data-action="remove-link" data-id="${it.id}" data-field="${field.id}" data-idx="${idx}">✕</button>` : ""}
				</a>`).join("") || (!state.editing ? `<div class="muted">没有链接</div>` : "")}
			${state.editing ? `
				<div class="link-add">
					<input data-role="link-title" data-field="${field.id}" placeholder="标题，如 Steam">
					<input data-role="link-url" data-field="${field.id}" placeholder="https://">
					<button class="btn" type="button" data-action="add-link" data-id="${it.id}" data-field="${field.id}">添加</button>
				</div>` : ""}
		</div>`;
	}

	function renderDetail() {
		const pane = $("detail");
		const it = itemById(state.selectedId);
		if (!it) { pane.hidden = true; pane.innerHTML = ""; return; }
		pane.hidden = false;
		const g = groupById(it.groupId);
		const pathNames = ancestors(it.groupId).map((x) => x.name).join(" / ");
		const fields = it.fields || [];

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
					<div class="field-label">标签 <span class="field-type">只用于筛选</span></div>
					${state.editing ? `
						<div class="tag-edit">
							${(it.tags || []).map((t) => `
								<span class="tag-edit-chip">${esc(t)}<button type="button" data-action="remove-tag" data-id="${it.id}" data-tag="${esc(t)}">✕</button></span>
							`).join("")}
							<div class="tag-add">
								<input data-role="new-tag" placeholder="新标签">
								<button class="btn" type="button" data-action="add-tag" data-id="${it.id}">添加</button>
							</div>
						</div>` : `
						<div class="card-flags">${(it.tags || []).length
							? it.tags.map((t) => `<span class="chip tag">${esc(t)}</span>`).join("")
							: `<span class="muted">没有标签</span>`}</div>`}
				</div>
				${fields.map((field) => `
					<div class="field">
						<div class="field-label">
							${state.editing ? `
								<div class="field-label-edit">
									<input class="field-label-input" data-role="field-label" data-field="${field.id}" value="${esc(field.label)}">
									<span class="field-type">${typeLabel(field.type)}</span>
								</div>
								<button class="btn btn-ghost btn-danger" type="button" data-action="remove-field" data-id="${it.id}" data-field="${field.id}">删除</button>
							` : `<span>${esc(field.label)} · ${typeLabel(field.type)}</span>`}
						</div>
						${fieldBodyHTML(it, field)}
					</div>`).join("")}
				${state.editing ? `
					<div class="field">
						<div class="field-label">添加字段</div>
						<div class="add-field">
							<button class="btn" type="button" data-action="add-field" data-id="${it.id}" data-type="images">＋ 图片</button>
							<button class="btn" type="button" data-action="add-field" data-id="${it.id}" data-type="text">＋ 文字</button>
							<button class="btn" type="button" data-action="add-field" data-id="${it.id}" data-type="links">＋ 链接</button>
						</div>
					</div>` : (!fields.length ? `<div class="muted">还没有自定义字段。</div>` : "")}
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
		clearTimeout(state.toastTimer);
		state.toastTimer = setTimeout(() => { el.hidden = true; }, 2400);
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
		$("dialog").querySelector("input, select, textarea, button")?.focus();
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
			<p class="hint">只把路径记进软件在 LocalAppData 里的库。原目录不会被新建、修改或删除任何文件。</p>
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
			<p class="hint">分组可以嵌套。标签不会出现在这里，它只用来筛选。</p>
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
			<h3>原型 3</h3>
			<p class="hint">元数据只存在软件自己的 LocalAppData 库里，原目录保持原样。</p>
			<ul class="help-list">
				<li>每条可以自定义字段：图片、文字、链接，能增删和改名。</li>
				<li>标签只出现在左侧筛选，不参与分组树。</li>
				<li>完整度筛选、组内排序只影响中间列表，树的结构和计数不变。</li>
				<li>「从库中移除」只删库记录，不会动磁盘上的文件。</li>
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
		if (!path) { toast("先指定一个已有路径"); return; }
		if (!groupById(groupId)) { toast("先选一个分组"); return; }
		if (!name) {
			const parts = path.split(/[/\\]/).filter(Boolean);
			name = parts[parts.length - 1] || path;
		}
		const kind = kindOf(groupId);
		const created = {
			id: "n" + (++uid),
			groupId,
			name,
			path,
			exists: true,
			addedAt: new Date().toISOString().slice(0, 10),
			tags: [],
			kicker: groupById(groupId)?.name || "",
			pattern: kind === "music" ? "vinyl" : kind === "video" ? "stage" : "embers",
			h: Math.floor(Math.random() * 360),
			title: name.toUpperCase(),
			ratio: kind === "music" ? "square" : kind === "video" ? "wide" : "portrait",
			fields: defaultFields(),
		};
		items.unshift(created);
		selectGroup(groupId);
		state.selectedId = created.id;
		state.editing = true;
		closeDialog();
		renderAll();
		toast("已写入本机库，原目录没有改动");
	}

	function confirmAddGroup() {
		const name = $("newGroupName").value.trim();
		if (!name) { toast("给分组起个名字"); return; }
		const parentId = $("newGroupParent").value || null;
		const id = "g" + (++uid);
		const parent = parentId ? groupById(parentId) : null;
		groups.push({ id, name, parentId, kind: parent ? kindOf(parent) : "games" });
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
		if (childrenOf(id).length) { toast("先删掉或移走子分组"); return; }
		const owned = items.filter((it) => it.groupId === id);
		if (owned.length) {
			if (!confirm(`「${g.name}」里还有 ${owned.length} 个条目。\n删除分组后，它们会移到上一层。继续？`)) return;
			if (!g.parentId) { toast("顶级分组里还有条目，先移走再删"); return; }
			owned.forEach((it) => { it.groupId = g.parentId; });
		} else if (!confirm(`删除分组「${g.name}」？`)) return;
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
		if (!it.exists) toast("路径已经找不到。库还在 LocalAppData，原目录没有被这个软件改过。");
		else toast(`原型：将打开\n${it.path}\n不会对其中文件做任何写入。`);
	}
	function openShot(itemId, fieldId, shotId) {
		const it = itemById(itemId);
		const field = (it?.fields || []).find((f) => f.id === fieldId);
		const s = field?.values?.find((x) => x.id === shotId);
		if (!s) return;
		$("lightboxStage").innerHTML = shotFace(s);
		$("lightboxCap").textContent = [it.name, field.label, s.caption].filter(Boolean).join(" · ");
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
		const sel = document.querySelector("[data-role='item-group']");
		if (sel && groupById(sel.value)) it.groupId = sel.value;
		document.querySelectorAll("[data-role='field-label']").forEach((input) => {
			const field = (it.fields || []).find((f) => f.id === input.dataset.field);
			if (field) field.label = input.value.trim() || field.label;
		});
		document.querySelectorAll("[data-role='field-text']").forEach((input) => {
			const field = (it.fields || []).find((f) => f.id === input.dataset.field);
			if (field) field.value = input.value;
		});
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
			case "toggle-tag": {
				const tag = hit.dataset.tag;
				if (state.tagFilters.has(tag)) state.tagFilters.delete(tag);
				else state.tagFilters.add(tag);
				renderAll();
				break;
			}
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
				openShot(hit.dataset.item, hit.dataset.field, hit.dataset.shot);
				break;
			case "close-lightbox":
				$("lightbox").hidden = true;
				break;
			case "pick-images":
				state.pendingImages = { itemId: hit.dataset.id, fieldId: hit.dataset.field };
				$("imagePicker").click();
				break;
			case "add-field": {
				const it = itemById(hit.dataset.id);
				if (!it) break;
				it.fields = it.fields || [];
				const type = hit.dataset.type;
				const label = type === "images" ? "图片" : type === "text" ? "文字" : "链接";
				it.fields.push(type === "text"
					? { id: fid(), type, label, value: "" }
					: { id: fid(), type, label, values: [] });
				renderDetail();
				break;
			}
			case "remove-field": {
				const it = itemById(hit.dataset.id);
				if (!it) break;
				it.fields = (it.fields || []).filter((f) => f.id !== hit.dataset.field);
				renderDetail();
				break;
			}
			case "add-link": {
				const it = itemById(hit.dataset.id);
				const field = (it?.fields || []).find((f) => f.id === hit.dataset.field);
				const box = hit.closest(".link-add") || document;
				const title = box.querySelector("[data-role='link-title']")?.value.trim();
				const url = box.querySelector("[data-role='link-url']")?.value.trim();
				if (!field || !url) { toast("链接地址不能空"); break; }
				field.values = field.values || [];
				field.values.push({ title: title || url, url });
				renderDetail();
				break;
			}
			case "remove-link": {
				const it = itemById(hit.dataset.id);
				const field = (it?.fields || []).find((f) => f.id === hit.dataset.field);
				if (!field) break;
				field.values.splice(Number(hit.dataset.idx), 1);
				renderDetail();
				break;
			}
			case "add-tag": {
				const it = itemById(hit.dataset.id);
				const input = document.querySelector("[data-role='new-tag']");
				const tag = input?.value.trim();
				if (!it || !tag) { toast("标签不能空"); break; }
				it.tags = it.tags || [];
				if (!it.tags.includes(tag)) it.tags.push(tag);
				renderAll();
				break;
			}
			case "remove-tag": {
				const it = itemById(hit.dataset.id);
				if (!it) break;
				it.tags = (it.tags || []).filter((t) => t !== hit.dataset.tag);
				renderAll();
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
	$("incompleteFilter").addEventListener("change", (e) => {
		state.incomplete = e.target.value;
		renderToolbar();
		renderLibrary();
	});
	$("sortBy").addEventListener("change", (e) => {
		state.sortBy = e.target.value;
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
			toast("只记录路径，不会读取或改写该目录。原型里写成 " + path);
		}
	});

	$("imagePicker").addEventListener("change", (e) => {
		const files = [...(e.target.files || [])];
		const pending = state.pendingImages;
		e.target.value = "";
		const it = pending && itemById(pending.itemId);
		const field = it && (it.fields || []).find((f) => f.id === pending.fieldId);
		if (!field || !files.length) return;
		field.values = field.values || [];
		files.forEach((file) => {
			const reader = new FileReader();
			reader.onload = () => {
				field.values.push({ id: iid(), src: reader.result, caption: file.name });
				if (state.selectedId === it.id) renderDetail();
				renderLibrary();
			};
			reader.readAsDataURL(file);
		});
	});

	renderAll();
})();
