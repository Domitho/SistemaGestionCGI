function UTC_FileInput(config) {

    const wrapper = document.getElementById(config.wrapper);
    const dropzone = document.getElementById(config.dropzone);
    const preview = document.getElementById(config.preview);
    const loader = document.getElementById(config.loader);
    const realInput = document.getElementById(config.input);

    if (!wrapper || !dropzone || !preview || !realInput) return;

    const renameBtn = wrapper.querySelector(".rename-btn");
    const removeBtn = wrapper.querySelector(".remove-btn");
    const renameField = wrapper.querySelector(".utc-edit-name-field");
    const fileNameLabel = wrapper.querySelector(".utc-fileinput-name");

    let pdfDoc = null;
    let currentPage = 1;
    let totalPages = 1;
    let scale = 1.0;

    let pdfViewer, pdfCanvas, pdfCtx, pdfPageLabel, pdfTotalLabel;

    function initPdfElements() {
        pdfViewer = wrapper.querySelector(".utc-pdf-viewer");
        pdfCanvas = wrapper.querySelector(".utc-pdf-canvas");
        pdfPageLabel = wrapper.querySelector(".utc-pdf-page");
        pdfTotalLabel = wrapper.querySelector(".utc-pdf-total");

        if (pdfCanvas)
            pdfCtx = pdfCanvas.getContext("2d");
    }

    async function renderPDFPage(pageNumber) {
        const page = await pdfDoc.getPage(pageNumber);
        const viewport = page.getViewport({ scale });

        pdfCanvas.width = viewport.width;
        pdfCanvas.height = viewport.height;

        const renderContext = {
            canvasContext: pdfCtx,
            viewport
        };

        await page.render(renderContext).promise;

        pdfPageLabel.textContent = pageNumber;
        pdfTotalLabel.textContent = totalPages;
    }

    async function showPDF(file) {
        preview.style.display = "block";
        preview.innerHTML = "";
        loader.style.display = "none";

        const viewer = document.createElement("div");
        viewer.innerHTML = `
            <div class="utc-pdf-viewer">
                <div class="utc-pdf-toolbar d-flex align-items-center justify-content-between mb-2">
                    <div class="d-flex align-items-center gap-1">
                        <button type="button" class="btn btn-sm btn-outline-primary utc-pdf-prev"><i class="fa-solid fa-chevron-left"></i></button>
                        <button type="button" class="btn btn-sm btn-outline-primary utc-pdf-next"><i class="fa-solid fa-chevron-right"></i></button>
                        <span class="ms-2 small">Página <span class="utc-pdf-page">1</span> de <span class="utc-pdf-total">1</span></span>
                    </div>
                    <div class="d-flex align-items-center gap-1">
                        <button type="button" class="btn btn-sm btn-outline-secondary utc-pdf-zoom-out"><i class="fa-solid fa-magnifying-glass-minus"></i></button>
                        <button type="button" class="btn btn-sm btn-outline-secondary utc-pdf-zoom-reset">100%</button>
                        <button type="button" class="btn btn-sm btn-outline-secondary utc-pdf-zoom-in"><i class="fa-solid fa-magnifying-glass-plus"></i></button>
                    </div>
                </div>
                <div class="utc-pdf-canvas-container">
                    <canvas class="utc-pdf-canvas"></canvas>
                </div>
            </div>
        `;

        preview.appendChild(viewer);
        initPdfElements();

        const arrayBuffer = await file.arrayBuffer();
        pdfDoc = await pdfjsLib.getDocument({ data: arrayBuffer }).promise;

        totalPages = pdfDoc.numPages;
        currentPage = 1;

        renderPDFPage(currentPage);

        viewer.querySelector(".utc-pdf-prev").onclick = () => {
            if (currentPage > 1) renderPDFPage(--currentPage);
        };

        viewer.querySelector(".utc-pdf-next").onclick = () => {
            if (currentPage < totalPages) renderPDFPage(++currentPage);
        };

        viewer.querySelector(".utc-pdf-zoom-in").onclick = () => {
            scale += 0.15;
            renderPDFPage(currentPage);
        };

        viewer.querySelector(".utc-pdf-zoom-out").onclick = () => {
            if (scale > 0.25) scale -= 0.15;
            renderPDFPage(currentPage);
        };

        viewer.querySelector(".utc-pdf-zoom-reset").onclick = () => {
            scale = 1.0;
            renderPDFPage(currentPage);
        };
    }

    function showImage(file) {
        loader.style.display = "none";
        preview.style.display = "block";
        preview.innerHTML = `
            <img class="utc-image-preview" src="${URL.createObjectURL(file)}">
        `;
    }

    function showZip(file, isFolder = false) {
        loader.style.display = "none";
        preview.style.display = "block";
        let label = isFolder ? "Carpeta / Múltiples Archivos" : file.name;
        let icon = isFolder ? "fa-folder-open" : "fa-file-zipper";

        preview.innerHTML = `
            <div class="utc-generic-preview p-3 border rounded bg-light text-center text-primary">
                <i class="fa-solid ${icon} fa-3x mb-2"></i><br>
                <strong>${label}</strong><br>
                <span class="small text-muted">${isFolder ? "Se comprimirá automáticamente" : "Archivo Comprimido"}</span>
            </div>
        `;
    }

    function showGeneric(file) {
        loader.style.display = "none";
        preview.style.display = "block";
        preview.innerHTML = `
            <div class="utc-generic-preview">
                <i class="fa-solid fa-file me-2"></i> ${file.name}
            </div>
        `;
    }

    function handleFile(file) {
        const isMultiple = realInput.files && realInput.files.length > 1;

        const extensionesPermitidas = /(\.pdf|\.doc|\.docx|\.zip|\.rar|\.7z)$/i;

        if (!isMultiple && !extensionesPermitidas.exec(file.name)) {
            if (typeof toastify === 'function') {
                toastify('ww', 'Formato no permitido. Solo PDF, WORD o ZIP.', 'Sistema');
            } else {
                alert('Formato no permitido. Solo PDF, WORD o ZIP.');
            }

            realInput.value = "";
            fileNameLabel.textContent = "Ningún archivo seleccionado";
            return;
        }

        dropzone.style.display = "none";
        loader.style.display = "block";

        if (isMultiple) {
            fileNameLabel.textContent = "Carpeta / Múltiples Archivos";
        } else {
            fileNameLabel.textContent = file.name;
        }

        setTimeout(() => {
            let ext = file.name.split('.').pop().toLowerCase();

            if (isMultiple) {
                showZip(file, true);
            }
            else if (file.type.includes("pdf") || ext === 'pdf') {
                showPDF(file);
            }
            else if (file.type.includes("image") || ['jpg', 'jpeg', 'png'].includes(ext)) {
                showImage(file);
            }
            else if (file.type.includes("zip") || file.type.includes("compressed") || ['zip', 'rar', '7z'].includes(ext)) {
                showZip(file, false);
            }
            else {
                showGeneric(file);
            }
        }, 400);
    }

    dropzone.addEventListener("click", () => realInput.click());

    dropzone.addEventListener("dragover", (e) => {
        e.preventDefault();
        dropzone.classList.add("dragover");
    });

    dropzone.addEventListener("dragleave", () => {
        dropzone.classList.remove("dragover");
    });

    dropzone.addEventListener("drop", (e) => {
        e.preventDefault();
        dropzone.classList.remove("dragover");

        const file = e.dataTransfer.files[0];
        if (file) {
            realInput.files = e.dataTransfer.files;
            handleFile(file);
        }
    });

    realInput.addEventListener("change", function () {
        if (this.files.length > 0) {
            handleFile(this.files[0]);
        }
    });

    renameBtn?.addEventListener("click", () => {
        renameField.style.display = "block";
        renameField.value = fileNameLabel.textContent.trim();
        renameField.focus();
    });

    renameField?.addEventListener("blur", () => {
        if (renameField.value.trim() !== "")
            fileNameLabel.textContent = renameField.value.trim();
        renameField.style.display = "none";
    });

    removeBtn?.addEventListener("click", () => {
        fileNameLabel.textContent = "Ningún archivo seleccionado";
        realInput.value = "";
        preview.style.display = "none";
        preview.innerHTML = "";
        dropzone.style.display = "block";
    });

}