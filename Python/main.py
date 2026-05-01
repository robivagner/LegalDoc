# main.py
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from analyzer import DocumentAnalyzer

app = FastAPI(title="LegalDoc AI Service")
analyzer = DocumentAnalyzer()

# Definim structura datelor de intrare
class AnalysisRequest(BaseModel):
    text: str

@app.get("/")
async def root():
    return {"message": "LegalDoc AI Service is running"}

@app.post("/analyze")
async def process_document(request: AnalysisRequest):
    if not request.text or len(request.text.strip()) < 10:
        raise HTTPException(
            status_code=400,
            detail="Textul documentului este prea scurt pentru a fi analizat."
        )

    try:
        result = await analyzer.analyze(request.text)
        return result
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)