# ============================================================
# Stage 1: Build/Install dependinte
# ============================================================
FROM python:3.11-slim AS builder
WORKDIR /app

# Instalam doar ce e necesar pentru compilare
RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    && rm -rf /var/lib/apt/lists/*

# Copiem requirements si instalam intr-un prefix izolat
COPY requirements.txt .
RUN pip install --upgrade pip && \
    pip install --prefix=/install --no-cache-dir -r requirements.txt

# ============================================================
# Stage 2: Runtime (imagine finala curata)
# ============================================================
FROM python:3.11-slim AS runtime
WORKDIR /app

# Copiem pachetele instalate din stage de build
COPY --from=builder /install /usr/local

# User non-root pentru securitate
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

# Copiem codul aplicatiei
COPY main.py .
COPY analyzer.py .

# Cream folder pentru documente procesate si dam permisiuni
RUN mkdir -p /app/documents && chown -R appuser:appgroup /app

USER appuser

EXPOSE 8000

# Uvicorn cu optimizari pentru productie
CMD ["uvicorn", "main:app", \
     "--host", "0.0.0.0", \
     "--port", "8000", \
     "--workers", "2", \
     "--loop", "uvloop", \
     "--access-log"]