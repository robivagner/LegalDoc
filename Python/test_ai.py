# test_ai.py
import asyncio
from analyzer import DocumentAnalyzer

async def test():
    try:
        a = DocumentAnalyzer()
        print("Inițializare reușită!")
        res = await a.analyze("Acesta este un contract de test pentru verificare API.")
        print("Rezultat AI:", res)
    except Exception as e:
        print("EROARE CRITICĂ:", e)

asyncio.run(test())