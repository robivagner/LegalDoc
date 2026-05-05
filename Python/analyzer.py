import os
import json
import asyncio
from google import genai
from dotenv import load_dotenv
from pathlib import Path

current_dir = Path(__file__).parent
env_path = current_dir.parent / '.env'

is_loaded = load_dotenv(dotenv_path=env_path, override=True)

class DocumentAnalyzer:
    def __init__(self):
        api_key = os.getenv("GEMINI_API_KEY")
        if not api_key:
            raise ValueError("GEMINI_API_KEY nu a fost găsit în .env")

        self.client = genai.Client(api_key=api_key)
        self.model_name = "gemini-3.1-flash-lite-preview"

    async def analyze(self, content: str):
        if not content or len(content.strip()) < 10:
            raise ValueError("Conținut insuficient pentru analiză.")

        prompt = f"""
        You are an expert Senior Legal Counsel with 20 years of experience in contract litigation. 
        Analyze the following legal document and provide a DETAILED, EXHAUSTIVE professional analysis.
        
        STRICT OUTPUT RULES:
        1. The response must be a VALID JSON object with keys: "summary", "clauses", "risks".
        2. Values for "clauses" and "risks" MUST be plain strings, formatted as detailed numbered lists using newline characters (\\n).
        3. Language: SAME as the document.
        4. VERBOSITY: Be extremely detailed. Do not summarize important legal nuances. 
        5. SELECTIVE BOILERPLATE: Ignore only standard addresses and formatting. DO NOT ignore legal definitions or standard clauses if they impose specific burdens.
        
        FIELDS TO EXTRACT:
        - summary: A comprehensive overview of the legal relationship created by this document (around 10-12 sentences). Focus on the core objective and the "quid pro quo".
        - clauses: Identify ALL "Active Obligations". For each, specify: What must be done, Who must do it, and the Deadline/Trigger. Be exhaustive.
        - risks: Identify all "High-Impact Issues" and hidden traps. Look for: unilateral termination rights, ambiguous phrasing, liability shifts, and budget caps. Explain WHY each is a risk.
        
        LEGAL DOCUMENT TO ANALYZE:
        ---
        {content}
        ---
        """

        max_retries = 3
        base_delay = 2

        for attempt in range(max_retries):
            try:
                response = await self.client.aio.models.generate_content(
                    model=self.model_name,
                    contents=prompt,
                    config={
                        'response_mime_type': 'application/json'
                    }
                )

                if not response.text:
                    raise Exception("AI-ul a returnat un răspuns gol.")

                # --- FIX SINTAXĂ: Totul pe un singur rând pentru a evita SyntaxError ---
                clean_text = response.text.strip()

                if clean_text.startswith("```json"):
                    clean_text = clean_text[7:]

                if clean_text.endswith("```"):
                    clean_text = clean_text[:-3]

                return json.loads(clean_text.strip())

            except Exception as e:
                error_msg = str(e)
                # Dacă modelul e ocupat, reîncercăm conform logicii de Exponential Backoff
                if "503" in error_msg or "429" in error_msg or "UNAVAILABLE" in error_msg:
                    if attempt < max_retries - 1:
                        wait_time = base_delay * (2 ** attempt)
                        print(f"Modelul {self.model_name} ocupat. Reîncercăm {attempt + 1}/{max_retries} în {wait_time}s...")
                        await asyncio.sleep(wait_time)
                        continue

                print(f"Eroare AI finală: {error_msg}")
                raise e