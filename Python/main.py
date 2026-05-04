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

@app.post("/analyze")
async def process_document(request: AnalysisRequest):
    # Verificăm dacă am primit conținut
    if not request.content or len(request.content.strip()) < 10:
        raise HTTPException(
            status_code=400,
            detail="Conținutul documentului este prea scurt pentru a fi analizat."
        )

    try:
        # Trimitem content către analizator
        result = await analyzer.analyze(request.content)
        return result
    except Exception as e:
        print(f"CRASH: {str(e)}")
        raise HTTPException(status_code=500, detail=str(e))

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)