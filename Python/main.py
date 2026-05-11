from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from analyzer import DocumentAnalyzer

app = FastAPI(title="LegalDoc AI Service")
analyzer = DocumentAnalyzer()

# Definim structura datelor de intrare - Sincronizat cu .NET (Content)
class AnalysisRequest(BaseModel):
    content: str

@app.get("/")
async def root():
    return {"message": "LegalDoc AI Service is running"}

@app.post(
    "/analyze",
    responses={
        400: {"description": "Conținut prea scurt sau lipsă pentru analiză."},
        500: {"description": "Eroare internă în timpul procesării AI."}
    }
)
async def process_document(request: AnalysisRequest):
    # 1. Validare conținut
    if not request.content or len(request.content.strip()) < 10:
        raise HTTPException(
            status_code=400,
            detail="Conținutul documentului este prea scurt pentru a fi analizat."
        )

    try:
        # 2. Analiză AI
        result = await analyzer.analyze(request.content)
        return result

    except Exception as e:
        print(f"CRASH AI SERVICE: {str(e)}")
        # Documentăm și eroarea 500 pentru o transparență totală
        raise HTTPException(
            status_code=500,
            detail="Serviciul AI a întâmpinat o eroare neașteptată."
        )